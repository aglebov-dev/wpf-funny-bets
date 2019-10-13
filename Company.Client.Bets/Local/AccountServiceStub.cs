using System.Collections.Generic;
using Company.Client.Common.Interfaces;
using External.Types;

namespace Company.Client.Bets.Local
{
    class AccountServiceStub : IAccountIdentService
    {
        AccountInfo Account = new AccountInfo
        {
            AccountId = 777,
            Name = "Иван",
            Midname = "Сергеевич",
            Surname = "Пушкин"
        };

        public AccountInfo GetAccountByDocument(EPlayerType ePlayerType, PersonalDocumentType documentType, string serial, string number)
            => Account;

        public AccountInfo GetAccountByPhone(EPlayerType ePlayerType, string mobilePhone)
            => Account;

        public AccountInfo GetAccountByRFIDCard(string cardNumber)
            => Account;

        public IList<AccountInfo> GetAccountsByDocument(PersonalDocumentType type, string serial, string number)
            => new[] { Account };

        public AccountInfo GetAccountsByID(int id)
            => Account;

        public IList<PersonalDocumentDescription> GetPrimaryPersonalDocuments()
            => new[]
            {
                    new PersonalDocumentDescription(PersonalDocumentType.PASSPORT, "ПАСПОРТ"),
                    new PersonalDocumentDescription(PersonalDocumentType.OTHER, "ИНОЙ"),
                    new PersonalDocumentDescription(PersonalDocumentType.FOREIGN_PASSPORT, "ЗАГРАН ПАСПОРТ")
            };
    }
}
