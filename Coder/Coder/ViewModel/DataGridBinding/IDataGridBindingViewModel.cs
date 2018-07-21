namespace Xarivu.Coder.ViewModel.DataGridBinding
{
    public interface IDataGridBindingViewModel
    {
        bool HasValidationMessages { get; set; }
        bool HasChanges { get; set; }
        bool IsEditing { get; set; }
    }
}
