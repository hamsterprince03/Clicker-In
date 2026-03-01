using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClickerIn.Models;

namespace ClickerIn.Services
{
    public interface IScenarioChainRunner
    {
        Task RunChain(ScenarioChain chain, List<Scenario> allScenarios,
            CancellationToken ct, IProgress<string> progress = null);
        bool IsRunning { get; }
    }

    public sealed class ScenarioChainRunner : IScenarioChainRunner
    {
        private readonly IScenarioRunner _runner;
        private readonly ILogService _log;

        public bool IsRunning { get; private set; }

        public ScenarioChainRunner(IScenarioRunner runner, ILogService log)
        {
            _runner = runner;
            _log = log;
        }

        public async Task RunChain(ScenarioChain chain, List<Scenario> allScenarios,
            CancellationToken ct, IProgress<string> progress = null)
        {
            IsRunning = true;
            var enabledItems = chain.Items.Where(x => x.Enabled).OrderBy(x => x.Order).ToList();
            _log.Info("🔗 Цепочка \"" + chain.Name + "\": " + enabledItems.Count + " сценариев");

            try
            {
                for (int i = 0; i < enabledItems.Count; i++)
                {
                    ct.ThrowIfCancellationRequested();
                    var item = enabledItems[i];
                    var scenario = allScenarios.FirstOrDefault(s => s.Id == item.ScenarioId);

                    if (scenario == null)
                    {
                        _log.Warn("⚠ Сценарий не найден: " + item.ScenarioName);
                        if (chain.StopOnError) break;
                        continue;
                    }

                    string msg = "▶ [" + (i + 1) + "/" + enabledItems.Count + "] " + scenario.Name;
                    _log.Info(msg);
                    if (progress != null) progress.Report(msg);

                    try
                    {
                        await _runner.Run(scenario, ct);
                        _log.Info("✅ Завершён: " + scenario.Name);
                    }
                    catch (OperationCanceledException) { throw; }
                    catch (Exception ex)
                    {
                        _log.Error("❌ Ошибка: " + scenario.Name + " — " + ex.Message);
                        if (chain.StopOnError)
                        {
                            _log.Info("🛑 Цепочка остановлена из-за ошибки");
                            break;
                        }
                    }

                    if (i < enabledItems.Count - 1 && item.DelayAfterMs > 0)
                    {
                        _log.Info("⏳ Пауза " + item.DelayAfterMs + " мс...");
                        await Task.Delay(item.DelayAfterMs, ct);
                    }
                }

                _log.Info("🔗 Цепочка \"" + chain.Name + "\" завершена");
            }
            finally
            {
                IsRunning = false;
            }
        }
    }
}