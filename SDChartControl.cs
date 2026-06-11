using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SDChartControl
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("DA0BC432-AB25-4A1E-BBF5-608E0A084934")]
    [ComSourceInterfaces(typeof(ISDChartControlEvents))]
    [ProgId("SDChartControl.SDChartControl")]
    public partial class SDChartControl : UserControl, ISDChartControl
    {
        #region Fields

        private Chart _chart;
        private string _chartType = "Column";
        private string _lastError = string.Empty;
        private bool _pieExploded = false;
        private bool _showLabels = true;
        private int _pieExplodeThreshold = 20;

        #endregion

        #region COM Event Delegates

        public delegate void ChartRenderedDelegate();
        public delegate void SaveCompletedDelegate(string filePath, bool success);
        public delegate void DataPointClickedDelegate(string seriesName, string label, double value);

        #endregion

        #region COM Events

        public event ChartRenderedDelegate ChartRendered;
        public event SaveCompletedDelegate SaveCompleted;
        public event DataPointClickedDelegate DataPointClicked;

        #endregion

        #region Constructor

        /// <summary>
        /// Parameterless constructor required for COM.
        /// CRITICAL: Do NOT add child controls here — use OnHandleCreated.
        /// </summary>
        public SDChartControl()
        {
            _chartType = "Column";
            Size = new Size(600, 400);
            DoubleBuffered = true;
        }

        /// <summary>
        /// Safe place to initialize child controls — HWND exists here.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (!DesignMode)
                InitializeChart();
        }

        private void InitializeChart()
        {
            SuspendLayout();

            _chart = new Chart();
            _chart.Dock = DockStyle.Fill;
            _chart.BackColor = Color.White;

            ChartArea chartArea = new ChartArea("MainArea");
            chartArea.BackColor = Color.White;
            _chart.ChartAreas.Add(chartArea);

            Legend legend = new Legend("MainLegend");
            legend.Enabled = true;
            _chart.Legends.Add(legend);

            Title title = new Title("Gráfico");
            title.Font = new Font("Arial", 14F, FontStyle.Bold);
            _chart.Titles.Add(title);

            Series defaultSeries = new Series("Series1");
            defaultSeries.ChartType = SeriesChartType.Column;
            defaultSeries.ChartArea = "MainArea";
            defaultSeries.Legend = "MainLegend";
            _chart.Series.Add(defaultSeries);

            _chart.MouseClick += Chart_MouseClick;
            _chart.PostPaint += Chart_PostPaint;

            Controls.Add(_chart);

            ResumeLayout(false);
        }

        #endregion

        #region ISDChartControl — Chart Type

        public void SetChartType(string chartType)
        {
            if (InvokeRequired) { Invoke(new Action<string>(SetChartType), chartType); return; }
            try
            {
                _chartType = chartType ?? "Column";
                if (_chart == null) return;

                SeriesChartType type = ParseChartType(_chartType);
                foreach (Series s in _chart.Series)
                    s.ChartType = type;

                _chart.Invalidate();
            }
            catch (Exception ex) { _lastError = ex.Message; }
        }

        public string GetChartType()
        {
            return _chartType ?? string.Empty;
        }

        #endregion

        #region ISDChartControl — Titles

        public void SetTitle(string title)
        {
            if (InvokeRequired) { Invoke(new Action<string>(SetTitle), title); return; }
            try
            {
                if (_chart == null) return;
                if (_chart.Titles.Count > 0)
                    _chart.Titles[0].Text = title ?? string.Empty;
                else
                    _chart.Titles.Add(new Title(title ?? string.Empty));
                AdjustChartAreaForTitle(title);
                _chart.Invalidate();
            }
            catch (Exception ex) { _lastError = ex.Message; }
        }

        public void SetXAxisTitle(string title)
        {
            if (InvokeRequired) { Invoke(new Action<string>(SetXAxisTitle), title); return; }
            try
            {
                if (_chart?.ChartAreas.Count > 0)
                {
                    _chart.ChartAreas[0].AxisX.Title = title ?? string.Empty;
                    _chart.Invalidate();
                }
            }
            catch (Exception ex) { _lastError = ex.Message; }
        }

        public void SetYAxisTitle(string title)
        {
            if (InvokeRequired) { Invoke(new Action<string>(SetYAxisTitle), title); return; }
            try
            {
                if (_chart?.ChartAreas.Count > 0)
                {
                    _chart.ChartAreas[0].AxisY.Title = title ?? string.Empty;
                    _chart.Invalidate();
                }
            }
            catch (Exception ex) { _lastError = ex.Message; }
        }

        #endregion

        #region ISDChartControl — Data

        public void AddDataPoint(string label, double value)
        {
            if (InvokeRequired) { Invoke(new Action<string, double>(AddDataPoint), label, value); return; }
            try
            {
                if (_chart == null) return;
                if (_chart.Series.Count == 0)
                {
                    Series s = new Series("Series1");
                    s.ChartType = ParseChartType(_chartType);
                    s.ChartArea = "MainArea";
                    _chart.Series.Add(s);
                }
                DataPoint dp = new DataPoint();
                dp.AxisLabel = label ?? string.Empty;
                dp.YValues = new double[] { value };
                _chart.Series[0].Points.Add(dp);
                if (_pieExploded)
                    ApplyPieExploded(_chart.Series[0]);
                _chart.Invalidate();
            }
            catch (Exception ex) { _lastError = ex.Message; }
        }

        public void AddSeries(string seriesName)
        {
            if (InvokeRequired) { Invoke(new Action<string>(AddSeries), seriesName); return; }
            try
            {
                if (_chart == null || string.IsNullOrEmpty(seriesName)) return;
                if (_chart.Series.FindByName(seriesName) != null) return;

                Series series = new Series(seriesName);
                series.ChartType = ParseChartType(_chartType);
                series.ChartArea = "MainArea";
                series.Legend = "MainLegend";
                _chart.Series.Add(series);
            }
            catch (Exception ex) { _lastError = ex.Message; }
        }

        public void AddDataPointToSeries(string seriesName, string label, double value)
        {
            if (InvokeRequired) { Invoke(new Action<string, string, double>(AddDataPointToSeries), seriesName, label, value); return; }
            try
            {
                if (_chart == null) return;
                Series series = _chart.Series.FindByName(seriesName);
                if (series != null)
                {
                    DataPoint dp = new DataPoint();
                    dp.AxisLabel = label ?? string.Empty;
                    dp.YValues = new double[] { value };
                    series.Points.Add(dp);
                    if (_pieExploded)
                        ApplyPieExploded(series);
                    _chart.Invalidate();
                }
                else
                {
                    _lastError = $"Serie '{seriesName}' no encontrada. Use AddSeries primero.";
                }
            }
            catch (Exception ex) { _lastError = ex.Message; }
        }

        public void ClearData()
        {
            if (InvokeRequired) { Invoke(new Action(ClearData)); return; }
            try
            {
                if (_chart == null) return;
                foreach (Series s in _chart.Series)
                    s.Points.Clear();
                _chart.Invalidate();
            }
            catch (Exception ex) { _lastError = ex.Message; }
        }

        #endregion

        #region ISDChartControl — Visual

        public void SetBackgroundColor(string hexColor)
        {
            if (InvokeRequired) { Invoke(new Action<string>(SetBackgroundColor), hexColor); return; }
            try
            {
                Color color = ColorTranslator.FromHtml(hexColor);
                BackColor = color;
                if (_chart != null)
                {
                    _chart.BackColor = color;
                    if (_chart.ChartAreas.Count > 0)
                        _chart.ChartAreas[0].BackColor = color;
                    _chart.Invalidate();
                }
            }
            catch (Exception ex) { _lastError = ex.Message; }
        }

        public void SetTitleColor(string hexColor)
        {
            if (InvokeRequired) { Invoke(new Action<string>(SetTitleColor), hexColor); return; }
            try
            {
                Color color = ColorTranslator.FromHtml(hexColor);
                if (_chart?.Titles.Count > 0)
                {
                    _chart.Titles[0].ForeColor = color;
                    _chart.Invalidate();
                }
            }
            catch (Exception ex) { _lastError = ex.Message; }
        }

        public void SetSeriesColor(string seriesName, string hexColor)
        {
            if (InvokeRequired) { Invoke(new Action<string, string>(SetSeriesColor), seriesName, hexColor); return; }
            try
            {
                if (_chart == null) return;
                Color color = ColorTranslator.FromHtml(hexColor);

                Series target = string.IsNullOrEmpty(seriesName)
                    ? (_chart.Series.Count > 0 ? _chart.Series[0] : null)
                    : _chart.Series.FindByName(seriesName);

                if (target != null)
                {
                    target.Color = color;
                    _chart.Invalidate();
                }
            }
            catch (Exception ex) { _lastError = ex.Message; }
        }

        public void SetDataPointColor(string label, string hexColor)
        {
            if (InvokeRequired) { Invoke(new Action<string, string>(SetDataPointColor), label, hexColor); return; }
            try
            {
                if (_chart == null || string.IsNullOrEmpty(label)) return;
                Color color = ColorTranslator.FromHtml(hexColor);
                foreach (Series s in _chart.Series)
                    foreach (DataPoint dp in s.Points)
                        if (string.Equals(dp.AxisLabel, label, StringComparison.OrdinalIgnoreCase))
                            dp.Color = color;
                _chart.Invalidate();
            }
            catch (Exception ex) { _lastError = ex.Message; }
        }

        public void SetFontSize(int size)
        {
            if (InvokeRequired) { Invoke(new Action<int>(SetFontSize), size); return; }
            try
            {
                if (_chart == null || size <= 0) return;

                if (_chart.Titles.Count > 0)
                    _chart.Titles[0].Font = new Font("Arial", size, FontStyle.Bold);

                float labelSize = Math.Max(6, size - 2);

                if (_chart.ChartAreas.Count > 0)
                {
                    _chart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", labelSize);
                    _chart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Arial", labelSize);
                    _chart.ChartAreas[0].AxisX.TitleFont = new Font("Arial", labelSize);
                    _chart.ChartAreas[0].AxisY.TitleFont = new Font("Arial", labelSize);
                }

                // Also update series labels (applies to pie slice text, data point labels, etc.)
                foreach (Series s in _chart.Series)
                    s.Font = new Font("Arial", labelSize);

                if (_chart.Legends.Count > 0)
                    _chart.Legends[0].Font = new Font("Arial", labelSize);

                _chart.Invalidate();
            }
            catch (Exception ex) { _lastError = ex.Message; }
        }

        public void SetShowLabels(bool show)
        {
            if (InvokeRequired) { Invoke(new Action<bool>(SetShowLabels), show); return; }
            _showLabels = show;
            try
            {
                if (_chart == null) return;
                foreach (Series s in _chart.Series)
                    ApplyLabelSettings(s);
                _chart.Invalidate();
            }
            catch (Exception ex) { _lastError = ex.Message; }
        }

        public void SetPieExploded(bool exploded)
        {
            if (InvokeRequired) { Invoke(new Action<bool>(SetPieExploded), exploded); return; }
            _pieExploded = exploded;
            try
            {
                if (_chart == null) return;
                foreach (Series s in _chart.Series)
                    ApplyPieExploded(s);
                _chart.Invalidate();
            }
            catch (Exception ex) { _lastError = ex.Message; }
        }

        public void SetPieExplodeThreshold(int percent)
        {
            if (InvokeRequired) { Invoke(new Action<int>(SetPieExplodeThreshold), percent); return; }
            if (percent < 1) percent = 1;
            if (percent > 99) percent = 99;
            _pieExplodeThreshold = percent;
            try
            {
                if (_chart == null || !_pieExploded) return;
                foreach (Series s in _chart.Series)
                    ApplyPieExploded(s);
                _chart.Invalidate();
            }
            catch (Exception ex) { _lastError = ex.Message; }
        }




        private void ApplyPieExploded(Series s)
        {
            if (!_pieExploded)
            {
                foreach (DataPoint dp in s.Points)
                    dp["Exploded"] = "false";
                return;
            }

            double total = 0;
            foreach (DataPoint dp in s.Points)
                if (dp.YValues.Length > 0) total += dp.YValues[0];

            double threshold = _pieExplodeThreshold / 100.0;
            foreach (DataPoint dp in s.Points)
            {
                double pct = (total > 0) ? dp.YValues[0] / total : 0;
                dp["Exploded"] = (pct < threshold) ? "true" : "false";
            }
        }

        private void AdjustChartAreaForTitle(string title)
        {
            if (_chart == null || _chart.Titles.Count == 0) return;
            _chart.Titles[0].Visible = !string.IsNullOrWhiteSpace(title);
        }

        private void ApplyLabelSettings(Series s)
        {
            if (!_showLabels)
            {
                s.Label = string.Empty;
                s.IsValueShownAsLabel = false;
                // PieLabelStyle must be disabled explicitly — MSChart ignores Label="" for pie/doughnut
                if (s.ChartType == SeriesChartType.Pie || s.ChartType == SeriesChartType.Doughnut)
                    s["PieLabelStyle"] = "Disabled";
            }
            else
            {
                if (s.ChartType == SeriesChartType.Pie || s.ChartType == SeriesChartType.Doughnut)
                {
                    s["PieLabelStyle"] = "Outside";
                    s.Label = "#AXISLABEL (#PERCENT{P0})";
                }
                else
                {
                    s.IsValueShownAsLabel = true;
                }
            }
        }

        #endregion

        #region ISDChartControl — Export

        public void SaveAsJpeg(string filePath)
        {
            if (InvokeRequired) { Invoke(new Action<string>(SaveAsJpeg), filePath); return; }
            try
            {
                if (_chart == null || string.IsNullOrEmpty(filePath))
                {
                    _lastError = "Ruta de archivo vacía o control no inicializado";
                    RaiseSaveCompleted(filePath ?? string.Empty, false);
                    return;
                }

                string dir = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                _chart.SaveImage(filePath, ChartImageFormat.Jpeg);
                RaiseSaveCompleted(filePath, true);
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                RaiseSaveCompleted(filePath ?? string.Empty, false);
            }
        }

        #endregion

        #region ISDChartControl — Status / Info

        public string GetLastError()
        {
            return _lastError ?? string.Empty;
        }

        public void About()
        {
            try
            {
                var asm = System.Reflection.Assembly.GetExecutingAssembly();
                var ver = asm.GetName().Version;
                MessageBox.Show(
                    $"SDChartControl\nVersión: {ver.Major}.{ver.Minor}.{ver.Build}\n\n" +
                    "Control de gráficos estadísticos para Clarion.\n" +
                    "Tipos soportados: Barras, Columnas, Torta, Líneas, Área, Doughnut, Spline.\n" +
                    "Export a JPEG incluido.\n\n" +
                    "ProgID: SDChartControl.SDChartControl",
                    "Acerca de SDChartControl",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch { }
        }

        #endregion

        #region Event Raising

        private void RaiseChartRendered()
        {
            if (ChartRendered != null)
                try { ChartRendered(); } catch { }
        }

        private void RaiseSaveCompleted(string filePath, bool success)
        {
            if (SaveCompleted != null)
                try { SaveCompleted(filePath, success); } catch { }
        }

        private void RaiseDataPointClicked(string seriesName, string label, double value)
        {
            if (DataPointClicked != null)
                try { DataPointClicked(seriesName, label, value); } catch { }
        }

        #endregion

        #region Internal Handlers

        private void Chart_PostPaint(object sender, ChartPaintEventArgs e)
        {
            if (e.ChartElement is Chart)
                RaiseChartRendered();
        }

        private void Chart_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                HitTestResult result = _chart.HitTest(e.X, e.Y);
                if (result.ChartElementType == ChartElementType.DataPoint && result.Series != null)
                {
                    DataPoint dp = result.Series.Points[result.PointIndex];
                    string label = dp.AxisLabel ?? result.PointIndex.ToString();
                    double value = dp.YValues.Length > 0 ? dp.YValues[0] : 0;
                    RaiseDataPointClicked(result.Series.Name, label, value);
                }
            }
            catch { }
        }

        #endregion

        #region Helpers

        private SeriesChartType ParseChartType(string chartType)
        {
            switch ((chartType ?? "Column").ToLowerInvariant())
            {
                case "bar":        return SeriesChartType.Bar;
                case "column":     return SeriesChartType.Column;
                case "pie":        return SeriesChartType.Pie;
                case "line":       return SeriesChartType.Line;
                case "area":       return SeriesChartType.Area;
                case "doughnut":   return SeriesChartType.Doughnut;
                case "spline":     return SeriesChartType.Spline;
                case "splinearea": return SeriesChartType.SplineArea;
                default:           return SeriesChartType.Column;
            }
        }

        #endregion

        #region Cleanup

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _chart?.Dispose();
                _chart = null;
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
