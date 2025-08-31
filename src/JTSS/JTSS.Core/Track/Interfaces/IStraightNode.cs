using JTSS.Core.Track.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTSS.Core.Track.Interfaces;

public interface IStraightNode : ITrackNode
{
    void Connect(TrackConnection connectionA, TrackConnection connectionB);
}
