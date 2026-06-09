using System.Runtime.InteropServices;

namespace SDChartControl
{
    /// <summary>
    /// COM events interface for SDChartControl.
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("22E789BA-81EF-4CEA-9166-DB0151F67728")]
    public interface ISDChartControlEvents
    {
        /// <summary>Fired after the chart is rendered</summary>
        [DispId(1)]
        void ChartRendered();

        /// <summary>Fired when SaveAsJpeg completes</summary>
        [DispId(2)]
        void SaveCompleted(string filePath, bool success);

        /// <summary>Fired when the user clicks on a data point</summary>
        [DispId(3)]
        void DataPointClicked(string seriesName, string label, double value);
    }
}
