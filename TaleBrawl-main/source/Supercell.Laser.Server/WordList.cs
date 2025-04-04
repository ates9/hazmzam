using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supercell.Laser.Server
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;

    public class Wordlist
    {
        private readonly string _filePath;
        private List<string> _words;

        public Wordlist(string filePath)
        {
            _filePath = filePath;
            LoadWords();
        }

        private void LoadWords()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var data = JsonConvert.DeserializeObject<WordData>(json);
                _words = data?.Words ?? new List<string>();
            }
            else
            {
                _words = new List<string>();
            }
        }

        private void SaveWords()
        {
            var data = new WordData { Words = _words };
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public bool AddWord(string word)
        {
            if (WordExists(word))
                return false;

            _words.Add(word);
            SaveWords();
            return true;
        }

        public bool DeleteWord(string word)
        {
            if (!_words.Remove(word))
                return false;

            SaveWords();
            return true;
        }

        public bool WordExists(string word)
        {
            return _words.Contains(word);
        }

        public List<string> GetWordList()
        {
            return new List<string>(_words);
        }

        public bool ContainsBlockedWord(string input)
        {
            foreach (var word in _words)
            {
                if (input.Contains(word))
                    return true;
            }
            return false;
        }

        private class WordData
        {
            public List<string> Words { get; set; }
        }
    }

}
