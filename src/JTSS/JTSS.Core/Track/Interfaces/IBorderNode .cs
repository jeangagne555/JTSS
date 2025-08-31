using JTSS.Core.Track.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JTSS.Core.Track.Interfaces;

/// <summary>
/// Defines a border node, which represents an entry or exit point of the track network.
/// It can only have a single connection.
/// </summary>
public interface IBorderNode : ITrackNode
{
    /// <summary>
    /// Connects a single track segment to this border node.
    /// </summary>
    /// <param name = "connection" > The track segment and end to connect.</param>
    void Connect(TrackConnection connection);
}
