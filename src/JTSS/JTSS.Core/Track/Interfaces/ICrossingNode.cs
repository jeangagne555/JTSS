using JTSS.Core.Track.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTSS.Core.Track.Interfaces;

public interface ICrossingNode : ITrackNode
{
    void Connect(TrackConnection line1A, TrackConnection line1B, TrackConnection line2A, TrackConnection line2B);
}
