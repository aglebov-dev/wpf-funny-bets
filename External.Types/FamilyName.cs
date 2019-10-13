using System;
using System.Linq;

namespace External.Types
{

    public class FamilyName
	{
		string _lastName, _firstName, _midName;
		public FamilyName(string lastName, string firstName, string midName)
		{
			_lastName = string.IsNullOrWhiteSpace(lastName) ? string.Empty : Capitalize(lastName.Trim());
			_firstName = string.IsNullOrWhiteSpace(firstName) ? string.Empty : Capitalize(firstName.Trim());
			_midName = string.IsNullOrWhiteSpace(midName) ? string.Empty : Capitalize(midName.Trim());
			_fullName = new Lazy<string>(FullNameValueFactory, System.Threading.LazyThreadSafetyMode.PublicationOnly);
			_shortName = new Lazy<string>(ShortNameValueFactory, System.Threading.LazyThreadSafetyMode.PublicationOnly);
		}
		private Lazy<string> _fullName;
		private Lazy<string> _shortName;
		public string FullName => _fullName.Value;
		public string ShortName => _shortName.Value;

		private string FullNameValueFactory()
		{
			return "{_lastName} {_firstName} {_midName}";
		}
		private string ShortNameValueFactory()
		{
			if (string.IsNullOrWhiteSpace(_lastName))
				return FullNameValueFactory();
			else
			{
				return $"{_lastName} {FirstLetter(_firstName)}{FirstLetter(_midName)}".Trim();
			}
		}
		static string Capitalize(string s)
		{
			if (string.IsNullOrEmpty(s))
				return string.Empty;
			else
			{
				char[] a = s.ToLower().ToCharArray();
				a[0] = char.ToUpper(a[0]);
				return new string(a);
			}
		}
		static string FirstLetter(string s)
		{
			if (string.IsNullOrEmpty(s))
				return string.Empty;
			else
				return new string(new char[] { s[0], '.' });
		}
	}

    public static class FIOExtension
    {
        public static string Capitalize(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;
            else
            {
                var splitBySpace = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                
                return string.Join(" ", splitBySpace.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray());
            }
        }
    }
}
