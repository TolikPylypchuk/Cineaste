global using System.Collections.Immutable;
global using System.Diagnostics.CodeAnalysis;
global using System.Text.Json;

global using Cineaste.Basic;
global using Cineaste.Core;
global using Cineaste.Core.Domain;
global using Cineaste.Persistence;
global using Cineaste.Server.Data;
global using Cineaste.Server.Exceptions;
global using Cineaste.Server.Mapping;
global using Cineaste.Server.Services;
global using Cineaste.Shared.Models;
global using Cineaste.Shared.Models.Franchise;
global using Cineaste.Shared.Models.List;
global using Cineaste.Shared.Models.Movie;
global using Cineaste.Shared.Models.Series;
global using Cineaste.Shared.Models.Shared;
global using Cineaste.Shared.Validation;

global using FluentValidation;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
