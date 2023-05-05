namespace Maintenance.Core.ViewModels
{
    public class PagingResultViewModel<TViewModel> where TViewModel : IBaseViewModel
    {
        public MetaViewModel Meta { get; set; }
        public List<TViewModel> Data { get; set; }
    }
    public class MetaViewModel
    {
        public int Pages { get => Convert.ToInt32(Math.Ceiling(Total / (float)Perpage)); }
        public int Page { get; set; }
        public int Perpage { get; set; }
        public int Total { get; set; }
    }
}
