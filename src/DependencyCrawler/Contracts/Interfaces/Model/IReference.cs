﻿using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Contracts.Interfaces.Model;

internal interface IReference : IReadOnlyReference
{
	public IProject Using { get; set; }
	public IProject UsedBy { get; set; }
	public ReferenceType ReferenceType { get; }
}