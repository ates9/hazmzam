namespace Supercell.Laser.Logic.Stream
{
    using Newtonsoft.Json;
    using Supercell.Laser.Logic.Club;
    using Supercell.Laser.Logic.Stream.Entry;

    public class AllianceStream
    {
        public const int MAX_STREAM_ENTRY_COUNT = 200;

        [JsonProperty("entry_id_counter")] public long EntryIdCounter;
        [JsonProperty("entries")] public List<AllianceStreamEntry> StreamEntryList;

        public AllianceStream()
        {
            StreamEntryList = new List<AllianceStreamEntry>();
        }

        public AllianceStreamEntry SendChatMessage(AllianceMember author, string content)
        {
            AllianceStreamEntry entry = new AllianceStreamEntry();
            entry.AuthorId = author.AccountId;
            if (!author.Avatar.IsDev)
            {
                entry.AuthorName = author.DisplayData.Name;
            }

            else
            {
                entry.AuthorName = author.DisplayData.Name + "atşw| DEVELOPER";
                // entry.AuthorName = author.DisplayData.Name + "<c00e7ff>atwş <c00e3ff>|<c00dfff> <c00daff>D<c00d6ff>E<c00d1fe>V<c00d5ff>E<c00d8ff>L<c00dcff>O<c00dfff>P<c00e2ff>E<c00e5fe>R</c>";
            }
            entry.AuthorRole = author.Role;
            entry.Id = ++EntryIdCounter;
            entry.Type = 2;
            entry.Message = content;
            AddEntry(entry);

            return entry;
        }

        public void AddEntry(AllianceStreamEntry entry)
        {
            if (StreamEntryList.Count >= MAX_STREAM_ENTRY_COUNT)
            {
                StreamEntryList.Remove(StreamEntryList[0]);
            }
            StreamEntryList.Add(entry);
        }

        public AllianceStreamEntry[] GetEntries()
        {
            return StreamEntryList.ToArray();
        }
    }
}
