﻿using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Data.Contracts.Entities;
using DependencyCrawler.Data.Contracts.Enum;

namespace DependencyCrawler.Implementations.Models.LinkedTypes;

internal class ProjectReference : Entity, IProjectReference
{
	public required IProject Using { get; set; }
	public required IProject UsedBy { get; set; }
	public ReferenceType ReferenceType => ReferenceType.Project;
	public IReadOnlyProject UsingReadOnly => Using;
	public IReadOnlyProject UsedByReadOnly => UsedBy;
	public ReferenceType ReferenceTypeReadOnly => ReferenceType;
}