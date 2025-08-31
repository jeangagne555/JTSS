using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTSS.Core.Track.Interfaces;

public interface ISwitchNode : ITrackNode
{
    ITrackSegment Facing { get; }
    ITrackSegment Trailing { get; }
    ITrackSegment Diverging { get; }
    SwitchState State { get; set; }
    void Connect(TrackConnection facing, TrackConnection trailing, TrackConnection diverging);
}
