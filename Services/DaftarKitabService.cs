using System.Collections.ObjectModel;
using Alkitab.Models;

namespace Alkitab.Services;

public class BibleInstancesService
{
    public BibleInstances BibleInstances { get; } = new BibleInstances();

    private static BibleInstancesService? _instance;
    public static BibleInstancesService Instance => _instance ??= new BibleInstancesService();

    private BibleInstancesService() {}
}