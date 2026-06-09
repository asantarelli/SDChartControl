# SDChartControl

Statistical Chart COM Control for Clarion — Bar, Pie, Line, Area, Doughnut, Spline charts with JPEG export.

A registration-free COM control built on .NET Framework 4.7.2 using `System.Windows.Forms.DataVisualization.Charting`. Drop the DLL and manifest next to your Clarion executable — no registry modifications required.

## Requirements

- .NET Framework 4.7.2 or later
- Clarion 11 or later (x86 application)

## Deployment

Copy these two files to the same folder as your Clarion executable:

| File | Purpose |
|------|---------|
| `SDChartControl.dll` | The COM control |
| `SDChartControl.manifest` | RegFree COM activation (required) |

> **Note:** The manifest must be named `SDChartControl.manifest` — NOT `SDChartControl.dll.manifest`.

## Chart Types

| Value | Description | Multi-series |
|-------|-------------|:---:|
| `"Column"` | Vertical bars (default) | ✅ |
| `"Bar"` | Horizontal bars | ✅ |
| `"Pie"` | Circular chart | ❌ |
| `"Line"` | Line chart | ✅ |
| `"Area"` | Filled area | ✅ |
| `"Doughnut"` | Pie with hole | ❌ |
| `"Spline"` | Smoothed curve | ✅ |
| `"SplineArea"` | Smoothed filled area | ✅ |

## API Reference

### Methods

| Method | Parameters | Returns | Description |
|--------|-----------|---------|-------------|
| `SetChartType` | `chartType STRING` | — | Set chart type (see table above) |
| `GetChartType` | — | `STRING` | Get current chart type |
| `SetTitle` | `title STRING` | — | Set main chart title |
| `SetXAxisTitle` | `title STRING` | — | Set X axis label |
| `SetYAxisTitle` | `title STRING` | — | Set Y axis label |
| `AddDataPoint` | `label STRING, value REAL` | — | Add point to default series |
| `AddSeries` | `seriesName STRING` | — | Create a new named series |
| `AddDataPointToSeries` | `seriesName STRING, label STRING, value REAL` | — | Add point to a named series |
| `ClearData` | — | — | Remove all data points |
| `SetBackgroundColor` | `hexColor STRING` | — | Set background color (e.g. `#FFFFFF`) |
| `SetTitleColor` | `hexColor STRING` | — | Set title text color |
| `SetSeriesColor` | `seriesName STRING, hexColor STRING` | — | Set series color (empty name = first series) |
| `SetFontSize` | `size LONG` | — | Set base font size in points |
| `SaveAsJpeg` | `filePath STRING` | — | Export chart to JPEG file |
| `GetLastError` | — | `STRING` | Get last error message |
| `About` | — | — | Show version info dialog |

### Events

| Event | Parameters | Description |
|-------|-----------|-------------|
| `ChartRendered` | — | Fired after each chart render |
| `SaveCompleted` | `filePath STRING, success BYTE` | Fired when `SaveAsJpeg` completes |
| `DataPointClicked` | `seriesName STRING, label STRING, value REAL` | Fired on data point click |

## Usage Flow

1. `SetChartType("Column")` — choose chart type
2. `SetTitle("My Chart")` — set title
3. `AddDataPoint("Jan", 1500)` — add data points
4. `AddDataPoint("Feb", 1800)`
5. `SaveAsJpeg("C:\output\chart.jpg")` — optional export

**Multi-series:**
1. `AddSeries("2024")` — add extra series (Series1 exists by default)
2. `AddDataPointToSeries("Series1", "Jan", 1500)`
3. `AddDataPointToSeries("2024", "Jan", 1800)`

## COM Identifiers

| Identifier | Value |
|-----------|-------|
| ProgID | `SDChartControl.SDChartControl` |
| CLSID | `{DA0BC432-AB25-4A1E-BBF5-608E0A084934}` |
| Interface GUID | `{22FAC9B7-E57F-4CBD-AA73-91CF7F3D9BD0}` |
| TypeLib GUID | `{EAD4D0BA-A1DC-49C8-A9CF-7DBA487BB443}` |

## Building from Source

Requires Visual Studio 2022 (or MSBuild from VS).

```powershell
# Restore and build
MSBuild SDChartControl.csproj -t:Restore
MSBuild SDChartControl.csproj -p:Configuration=Release
```

After build, deployment files are auto-generated in `Clarion/accessory/`.

## License

MIT License — Copyright © 2026 SD Digitales
