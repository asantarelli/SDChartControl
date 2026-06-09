using System.Runtime.InteropServices;

namespace SDChartControl
{
    /// <summary>
    /// COM interface for the SDChartControl statistical chart control.
    /// Supports Bar, Column, Pie, Line, Area, Doughnut, and Spline chart types.
    /// </summary>
    [ComVisible(true)]
    [Guid("22FAC9B7-E57F-4CBD-AA73-91CF7F3D9BD0")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ISDChartControl
    {
        /// <summary>Sets the chart type. Values: Bar, Column, Pie, Line, Area, Doughnut, Spline, SplineArea</summary>
        [DispId(1)]
        void SetChartType(string chartType);

        /// <summary>Returns the current chart type name</summary>
        [DispId(2)]
        string GetChartType();

        /// <summary>Sets the main chart title</summary>
        [DispId(3)]
        void SetTitle(string title);

        /// <summary>Sets the X axis title label</summary>
        [DispId(4)]
        void SetXAxisTitle(string title);

        /// <summary>Sets the Y axis title label</summary>
        [DispId(5)]
        void SetYAxisTitle(string title);

        /// <summary>Adds a data point to the default series (Series1)</summary>
        [DispId(6)]
        void AddDataPoint(string label, double value);

        /// <summary>Adds a new named data series to the chart</summary>
        [DispId(7)]
        void AddSeries(string seriesName);

        /// <summary>Adds a data point to a specific named series</summary>
        [DispId(8)]
        void AddDataPointToSeries(string seriesName, string label, double value);

        /// <summary>Clears all data points from all series</summary>
        [DispId(9)]
        void ClearData();

        /// <summary>Sets the chart background color (hex, e.g. #FFFFFF)</summary>
        [DispId(10)]
        void SetBackgroundColor(string hexColor);

        /// <summary>Sets the title text color (hex)</summary>
        [DispId(11)]
        void SetTitleColor(string hexColor);

        /// <summary>Sets the color of a specific series (empty seriesName = first series)</summary>
        [DispId(12)]
        void SetSeriesColor(string seriesName, string hexColor);

        /// <summary>Sets the base font size for chart labels and title</summary>
        [DispId(13)]
        void SetFontSize(int size);

        /// <summary>Saves the chart as a JPEG image to the specified file path</summary>
        [DispId(14)]
        void SaveAsJpeg(string filePath);

        /// <summary>Returns the last error message, or empty string if no error</summary>
        [DispId(15)]
        string GetLastError();

        /// <summary>Shows control version and information</summary>
        [DispId(16)]
        void About();
    }
}
