using System;
using Company.Client.Bets.Interfaces.Observers;
using Company.Client.Bets.Models;

namespace Company.Client.Bets.Interfaces
{
    interface IDataService
    {
        /// <summary>
        /// Подписаться на уведомления по событиям
        /// </summary>
        IDisposable SubscribeOnEventsInfo(IEventInfoObserver observer);
        /// <summary>
        /// Подписаться на уведомления по тиражам суперэкспресса
        /// </summary>
        IDisposable SubscribeOnDrawsInfo(ISuperexpressDrawsInfoObserver observer);
        /// <summary>
        /// Подписаться на уведомления по конкретному событию (изменение статуса события, изменения в исходах событий)
        /// </summary>
        IDisposable SubscribeOnEvent(int eventID, IEventObserver observer);
        /// <summary>
        /// Подписаться на уведомления по конкретному тиражу суперэкспресса
        /// </summary>
        IDisposable SubscribeOnDraw(int id, ISuperexpressDrawObserver observer);
        /// <summary>
        /// Подписаться на уведомления по совершенным ставкам
        /// </summary>
        IDisposable SubscribeOnMadeBets(IMadeBetsObserver observer);
        /// <summary>
        /// Получить данные о ставке
        /// </summary>
        BetInfoModel GetMadeBetInfo(int betID);
    }
}
