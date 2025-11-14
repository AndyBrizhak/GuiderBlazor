using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiderBlazor.Shared.Services
{
    public interface ITagsService
    {
        /// <summary>
        /// Получает список активных тегов на основе текущих фильтров.
        /// </summary>
        /// <param name="category">Текущая выбранная категория (или null).</param>
        /// <param name="province">Текущая выбранная провинция (или null).</param>
        /// <param name="city">Текущий выбранный город (или null).</param>
        /// <param name="selectedTags">Список уже выбранных тегов для drill-down фильтрации (или null).</param>
        /// <returns>Список названий тегов.</returns>
        Task<List<string>> GetActiveTagsAsync(string? category, string? province, string? city, List<string>? selectedTags);
    }
}
