global using System.Collections.Immutable;
global using System.Collections.ObjectModel;
global using System.ComponentModel;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.Reactive;
global using System.Reactive.Concurrency;
global using System.Reactive.Disposables;
global using System.Reactive.Linq;
global using System.Reactive.Subjects;
global using System.Resources;

global using Akavache;

global using Cineaste.Core.Comparers;
global using Cineaste.Core.Data;
global using Cineaste.Core.Data.Models;
global using Cineaste.Core.Data.Services;
global using Cineaste.Core.DialogModels;
global using Cineaste.Core.ListItems;
global using Cineaste.Core.Models;
global using Cineaste.Core.Preferences;
global using Cineaste.Core.Theming;
global using Cineaste.Core.Validation;
global using Cineaste.Core.ViewModels.Filters;
global using Cineaste.Core.ViewModels.Forms;
global using Cineaste.Core.ViewModels.Forms.Base;
global using Cineaste.Core.ViewModels.Forms.Preferences;
global using Cineaste.Data;
global using Cineaste.Data.Models;
global using Cineaste.Data.Services;

global using DynamicData;
global using DynamicData.Aggregation;
global using DynamicData.Binding;

global using Nito.Comparers;

global using ReactiveUI;
global using ReactiveUI.Fody.Helpers;
global using ReactiveUI.Validation.Extensions;
global using ReactiveUI.Validation.Helpers;

global using Splat;

global using static Cineaste.Core.Constants;
global using static Cineaste.Core.ServiceUtil;
global using static Cineaste.Core.ViewModels.Filters.FilterOperation;
global using static Cineaste.Core.ViewModels.Filters.FilterType;
global using static Cineaste.Data.Constants;

global using Filter = System.Func<Cineaste.Core.ListItems.ListItem, bool>;
