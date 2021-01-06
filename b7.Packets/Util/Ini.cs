using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace b7.Util
{
    public class Ini : IDictionary<string, IniSection>
    {
        private static readonly char[] COMMENT_CHARS = new[] { ';', '#' };

        private readonly IDictionary<string, IniSection> _sections;

        public ICollection<string> Keys => _sections.Keys;
        public ICollection<IniSection> Values => _sections.Values;
        public int Count => _sections.Count;
        public bool IsReadOnly => false;

        public IniSection this[string key]
        {
            get => _sections[key];
            set => _sections[key] = value;
        }

        public bool ContainsKey(string key) => _sections.ContainsKey(key);

        public void Add(string key, IniSection value) => _sections.Add(key, value);

        public bool Remove(string key) => _sections.Remove(key);

        public bool TryGetValue(string key, out IniSection value) => _sections.TryGetValue(key, out value);

        public void Add(KeyValuePair<string, IniSection> item) => _sections.Add(item);

        public void Clear() => _sections.Clear();

        public bool Contains(KeyValuePair<string, IniSection> item) => _sections.Contains(item);

        public void CopyTo(KeyValuePair<string, IniSection>[] array, int arrayIndex) => _sections.CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<string, IniSection> item) => _sections.Remove(item);

        public IEnumerator<KeyValuePair<string, IniSection>> GetEnumerator() => _sections.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Ini()
        {
            _sections = new Dictionary<string, IniSection>(StringComparer.OrdinalIgnoreCase);
        }

        public static Ini Load(string path)
        {
            using StreamReader reader = new(path);

            Ini ini = new();
            IniSection? currentSection = null;

            string? line = null;
            int lineNumber = 0;

            while ((line = reader.ReadLine()?.Trim()) != null)
            {
                lineNumber++;

                if (string.IsNullOrWhiteSpace(line)) continue;

                if (COMMENT_CHARS.Any(c => line.StartsWith(c)))
                    continue;

                if (line.StartsWith('['))
                {
                    if (!line.EndsWith(']'))
                        throw new FormatException($"Invalid section on line {lineNumber}");

                    string sectionName = line[1..^1];

                    if (string.IsNullOrWhiteSpace(sectionName))
                        throw new Exception($"Empty section name on line {lineNumber}");

                    if (ini.ContainsKey(sectionName))
                        throw new Exception($"Duplicate section name on line {lineNumber}");

                    currentSection = new IniSection();
                    ini.Add(sectionName, currentSection);
                }
                else
                {
                    int index = line.IndexOf('=');
                    string? key = null, value = string.Empty;

                    if (index >= 0)
                    {
                        key = line[..index].Trim();
                        value = line[(index + 1)..].Trim();
                    }

                    if (string.IsNullOrWhiteSpace(key))
                        throw new FormatException($"Empty entry key on line {lineNumber}");

                    if (currentSection is null)
                        throw new FormatException($"Entry with no section on line {lineNumber}");

                    currentSection?.Add(new IniEntry(key, value));
                }
            }

            return ini;
        }
    }

    public class IniSection : IList<IniEntry>
    {
        private readonly List<IniEntry> list = new List<IniEntry>();

        public IniSection() { }

        public IniEntry this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        public int Count => list.Count;

        public bool IsReadOnly => false;

        public void Add(IniEntry item) => list.Add(item);

        public void Clear() => list.Clear();

        public bool Contains(IniEntry item) => list.Contains(item);

        public void CopyTo(IniEntry[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

        public IEnumerator<IniEntry> GetEnumerator() => list.GetEnumerator();

        public int IndexOf(IniEntry item) => list.IndexOf(item);

        public void Insert(int index, IniEntry item) => list.Insert(index, item);

        public bool Remove(IniEntry item) => list.Remove(item);

        public void RemoveAt(int index) => list.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
    }

    public class IniEntry
    {
        public string Key { get; }
        public string Value { get; set; }

        public IniEntry(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public IniEntry(string key)
            : this(key, string.Empty)
        { }

        public void Deconstruct(out string key, out string value)
        {
            key = Key;
            value = Value;
        }

        public override int GetHashCode() => (Key, Value).GetHashCode();

        public override bool Equals(object obj)
        {
            return
                obj is IniEntry other &&
                other.Key == Key &&
                other.Value == Value;
        }
    }
}
