using DebrisToys.UI.Control;
using ShimizuToolkit.HotkeyWinUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json.Serialization;
using Windows.System;

namespace DebrisToys.Global.Helper
{
    [JsonSerializable(typeof(bool))]
    [JsonSerializable(typeof(int))]
    [JsonSerializable(typeof(string))]
    [JsonSerializable(typeof(Dictionary<string, HotKeyInfo>))]
    [JsonSerializable(typeof(HotKeyInfo))]
    [JsonSerializable(typeof(List<VirtualKey>))]
    [JsonSerializable(typeof(HashSet<VirtualKey>))]
    [JsonSerializable(typeof(Dictionary<string, bool>))]
    [JsonSerializable(typeof(ObservableCollection<TargetAppListCardItemDTO>))]
    [JsonSerializable(typeof(List<TargetAppListCardItemDTO>))]
    public partial class AppJsonContext : JsonSerializerContext
    {
    }
}
