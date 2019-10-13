using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace External.Types
{
    public class PersonalDocumentDescription
    {
        public PersonalDocumentType Type { get; }
        public PersonalDocumentGroupType GroupType { get; }
        public string Name { get; }
        public PersonalDocumentDescription(PersonalDocumentType type, string name, PersonalDocumentGroupType groupType = PersonalDocumentGroupType.UNTYPED)
        {
            Type = type;
            Name = name;
            GroupType = groupType;
        }
    }

    public enum PersonalDocumentType
    {
        PASSPORT = 1,
        MILITARY_TICKET = 2,
        SAILOR_PASSPORT = 3,
        OTHER = 4,
        FOREIGN_PASSPORT = 5,
        FORCED_MIGRANT_CARD = 6,
        MIGRANT_PETITION = 7,
        TEMPORARY_ASYLUM_DOCUMENT = 8,
        RESIDENCE_PERMIT = 9,
        PERMISSION_FOR_TEMPORARY_RESIDENCE = 10,
        VISA = 11,
        RESIDENT = 12
    }
    public enum PersonalDocumentGroupType
    {
        UNTYPED = 1,
        PRIMARY = 2,
        FOREIGN_CITIZEN = 4,
        ADDITIONAL = 8
    }
}
