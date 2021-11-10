global using System.ComponentModel;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.Reactive;
global using System.Reactive.Disposables;
global using System.Reactive.Linq;
global using System.Reactive.Subjects;
global using System.Resources;

global using Akavache;

global using Avalonia;
global using Avalonia.Controls;
global using Avalonia.Controls.Primitives;
global using Avalonia.Input;
global using Avalonia.Interactivity;
global using Avalonia.Markup.Xaml;
global using Avalonia.Media;
global using Avalonia.Media.Imaging;
global using Avalonia.ReactiveUI;

global using Cineaste.Controls;
global using Cineaste.Converters;
global using Cineaste.Core;
global using Cineaste.Core.DialogModels;
global using Cineaste.Core.ListItems;
global using Cineaste.Core.Models;
global using Cineaste.Core.Preferences;
global using Cineaste.Core.Theming;
global using Cineaste.Core.ViewModels;
global using Cineaste.Core.ViewModels.Filters;
global using Cineaste.Core.ViewModels.Forms;
global using Cineaste.Core.ViewModels.Forms.Preferences;
global using Cineaste.Data;
global using Cineaste.Data.Models;
global using Cineaste.Infrastructure;
global using Cineaste.Properties;
global using Cineaste.State;
global using Cineaste.Theming;
global using Cineaste.Validation;
global using Cineaste.Views;

global using DynamicData;
global using DynamicData.Aggregation;
global using DynamicData.Binding;

global using FluentAvalonia.UI.Controls;

global using ReactiveUI;
global using ReactiveUI.Validation.Extensions;

global using Splat;

global using static Cineaste.Constants;
global using static Cineaste.Core.Constants;
global using static Cineaste.Core.ServiceUtil;
global using static Cineaste.Data.Constants;
global using static Cineaste.Data.ListSortOrder;
global using static Cineaste.Util;

global using Button = Avalonia.Controls.Button;
