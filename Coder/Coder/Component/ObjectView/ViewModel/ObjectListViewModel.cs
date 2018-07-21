using Xarivu.Coder.Model.Tracked;
using System.Collections.Generic;

namespace Xarivu.Coder.Component.ObjectView.ViewModel
{
    public class ObjectListViewModel : NotifyChangeBase
    {
        List<object> objectList;

        public ObjectListViewModel(IEnumerable<object> initialObjects = null)
        {
            this.objectList = new List<object>();
            if (initialObjects != null)
            {
                this.objectList.AddRange(initialObjects);
            }
        }

        #region SelectedItem
        object __SelectedItem;
        public object SelectedItem
        {
            get
            {
                return this.__SelectedItem;
            }

            set
            {
                if (this.__SelectedItem != value)
                {
                    this.__SelectedItem = value;
                    NotifyPropertyChanged(nameof(SelectedItem));
                }
            }
        }
        #endregion


        public IEnumerable<object> ObjectList
        {
            get { return objectList; }
        }

        public void AddToList(object obj)
        {
            this.objectList.Add(obj);
            NotifyPropertyChanged(nameof(ObjectList));
        }

        public void ClearList()
        {
            this.objectList.Clear();
            NotifyPropertyChanged(nameof(ObjectList));
        }
    }
}
