using System.Globalization;
using System.Windows.Forms;

namespace MagmaC3
{
    internal class TrackListView : ListView
    {
        public TrackListView()
        {
            Columns.Add("File", 200, HorizontalAlignment.Left);
            Columns.Add("Type", 50, HorizontalAlignment.Center);
            Columns.Add("Rate", 50, HorizontalAlignment.Center);
            Columns.Add("Chan", 50, HorizontalAlignment.Center);
            Columns.Add("Pan", 60, HorizontalAlignment.Center);
            Columns.Add("Vol", 60, HorizontalAlignment.Center);
        }

        public void RefreshTracks()
        {
            foreach (ListViewItem listViewItem in Items)
            {
                var trackInfo = (TrackInfo)listViewItem.Tag;
                if (!trackInfo.IsValid())
                    UGCDebug.Notify("Invalid Track in ListView: " + trackInfo.Filename + ".");
                listViewItem.Text = trackInfo.Filename;
                while (listViewItem.SubItems.Count < 6)
                    listViewItem.SubItems.Add("");
                listViewItem.SubItems[1].Text = trackInfo.TrackType;
                listViewItem.SubItems[2].Text = trackInfo.SampleRate.ToString(CultureInfo.InvariantCulture);
                switch (trackInfo.NumChannels)
                {
                    case 1:
                        listViewItem.SubItems[3].Text = "Mono";
                        listViewItem.SubItems[4].Text = trackInfo.PanLeft.ToString(CultureInfo.InvariantCulture);
                        listViewItem.SubItems[5].Text = trackInfo.VolLeft.ToString(CultureInfo.InvariantCulture);
                        continue;
                    case 2:
                        listViewItem.SubItems[3].Text = "Stereo";
                        listViewItem.SubItems[4].Text = trackInfo.PanLeft.ToString(CultureInfo.InvariantCulture) + " / " + trackInfo.PanRight.ToString(CultureInfo.InvariantCulture);
                        listViewItem.SubItems[5].Text = trackInfo.VolLeft.ToString(CultureInfo.InvariantCulture) + " / " + trackInfo.VolRight.ToString(CultureInfo.InvariantCulture);
                        continue;
                    default:
                        throw new MagmaException("Illegal number of channels: " + trackInfo.NumChannels.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        public void AddTrack(TrackInfo track)
        {
            if (!track.IsValid()) return;
            Items.Add(new ListViewItem(track.Filename)
                {
                    Tag = track
                });
            RefreshTracks();
        }

        public TrackInfo GetSelectedTrack()
        {
            return (TrackInfo)Items[SelectedIndices[0]].Tag;
        }

        public void RemoveSelectedTracks()
        {
            while (SelectedIndices.Count > 0)
                Items.RemoveAt(SelectedIndices[0]);
        }

        public void RemoveAllTracks()
        {
            Items.Clear();
        }
    }
}
