using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiderBlazor.Shared.Services
{
    public interface ICitiesService
    {
        /// <summary>
        /// Получает список активных городов на основе текущих фильтров.
        /// </summary>
        /// <param name="category">Текущая выбранная категория (или null).</param>
        /// <param name="province">Текущая выбранная провинция (или null).</param>
        /// <returns>Список названий городов.</returns>
        Task<List<string>> GetActiveCitiesAsync(string? category, string? province);
    }
}
