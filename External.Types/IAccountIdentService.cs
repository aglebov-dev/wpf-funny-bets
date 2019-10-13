using System.Collections.Generic;
using External.Types;

namespace Company.Client.Common.Interfaces
{
    /// <summary>
    /// Работа с аккаунтами
    /// </summary>
    public interface IAccountIdentService
    {
        AccountInfo GetAccountsByID(int id);
        AccountInfo GetAccountByPhone(EPlayerType ePlayerType, string mobilePhone);
        AccountInfo GetAccountByRFIDCard(string cardNumber);
        AccountInfo GetAccountByDocument(EPlayerType ePlayerType, PersonalDocumentType documentType, string serial, string number);
        IList<AccountInfo> GetAccountsByDocument(PersonalDocumentType type, string serial, string number);
        IList<PersonalDocumentDescription> GetPrimaryPersonalDocuments();
    }
}
