﻿using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Implementations.Models.LinkedTypes;

internal class UnresolvedReference : Entity, IReference
{
    public required IProject Using { get; set; }
    public required IProject UsedBy { get; set; }
    public ReferenceType ReferenceType => ReferenceType.Unknown;
    public IReadOnlyProject UsingReadOnly => Using;
    public IReadOnlyProject UsedByReadOnly => UsedBy;
    public ReferenceType ReferenceTypeReadOnly => ReferenceType;
}