﻿using WebModels.Models;

namespace AppView.Areas.Admin.Repository
{
    public interface IMauSacRepo
    {
        Task<List<MauSac>> GetAll();
        Task<MauSac?> GetByID(Guid id);
        Task<MauSac> Create(MauSac mauSac);
        Task<MauSac?> Update(Guid id, MauSac mauSac);
        Task<bool> Delete(Guid id);
        Task<string> Toggle(Guid id);
    }
}
