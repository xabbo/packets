using System;

namespace b7.Packets
{
    public class StructureItem
    {
        public TypeCode Type { get; init; }
        public int Offset { get; init; }
        public int Length { get; init; }
        public object? Value { get; init; }
        public string? Name { get; set; }

        public StructureItem() { }
    }
}
