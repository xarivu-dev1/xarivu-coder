# Coder

Library that enables rapid development of Windows desktop applications using Windows Presentation Framework (WPF) and the Model-View-ViewModel (MVVM) design pattern.

## Introduction

Two core components of the Coder library that enable rapid application development are the TrackedModelBase and TrackedListBase classes. Both classes encapsulate application level behavior and properties that enable you to quickly write code without having to write your own change tracking and synchronization logic. Derive your application level models and lists from these classes when you need change tracking support.

### TrackedModelBase<TId, TModel\>
File: Coder\\Model\\Tracked\\TrackedModelBase\.cs
  
The tracked application model base class that supports property change notification, includes common application metadata, and provides a unique ID for each model instance.

Create a new class derived from TrackedModelBase for all models that require change notification. Typically, this will include all view models, but in many cases the application models themselves need to support change notification since they may be modified by other views or lower level operations (i.e. storage or network operations).

The TId parameter is a class or struct that represents the unique ID for the model instance.
The TModel parameter is the raw model class with member properties with getters and setters.

The derived class should have member properties for all properties of the model that follow the getter/setter syntax for change notification. For example if the Model has a property:
```
    public string Name { get; set; }
```    
Then the derived Tracked Model class should have:
```
    #region Name
    string __Name;
    public string Name
    {
        get
        {
            return this.__Name;
        }

        set
        {
            if (this.__Name != value)
            {
                this.__Name = value;
                NotifyPropertyChanged();
            }
        }
    }
    #endregion
```

### TrackedModelGuidBase<TModel\>
File: Coder\\Model\\Tracked\\TrackedModelGuidBase\.cs

A base class that extends the TrackedModelBase and uses GUID as the ID type. Derive your Tracked Models from this class if your model does not have a unique ID.

### TrackedListBase<TId, TModel\>
File: Coder\\Model\\Tracked\\TrackedListBase\.cs

For collections that require change notification, derive from TrackedListBase. Internally the Track List wraps all elements in a Tracked Model and sends notification for addition, deletion, move, item property change, selection change. These changes can then be easily synchronized with WPF controls like DataGrid and ListBox.
