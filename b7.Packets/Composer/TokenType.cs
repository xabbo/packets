using System;

namespace b7.Packets.Composer
{
    /*
        true / false    Boolean
        1234            Integer
        1234l / 1234L   Long Integer
        1.234           Number
        "Text"          String
        [00 01 02]      Byte array

        -               Negate

        b:              Byte specifier
        s:              Short specifier
        i:              Int specifier
        l:              Long specifier

        abcd            Identifier
    */

    public enum TokenType
    {
        Undefined,

        // Types
        Boolean,
        Integer,
        LongInteger,
        Number,
        FloatingPointNumber,
        String,
        ByteArray,

        // Modifiers
        Negate,

        // Specifiers
        ByteSpecifier,
        ShortSpecifier,
        IntSpecifier,
        LongSpecifier,

        // Other
        Identifier,

        SequenceTerminator
    }
}
