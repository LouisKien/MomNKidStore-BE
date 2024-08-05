namespace MomNKidStore_BE.Business.BackgroundServices.Interfaces
{
    public interface IProductBackgroundService
    {
        Task RemoveHiddenProductInCustomerCarts();
    }
}
