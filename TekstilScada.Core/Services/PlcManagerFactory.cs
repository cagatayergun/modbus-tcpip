// File: TekstilScada.Core/Services/PlcManagerFactory.cs
using System;
using TekstilScada.Models;

namespace TekstilScada.Services
{
    /// <summary>
    /// Factory class that creates the appropriate IPlcManager object based on the given machine type.
    /// </summary>
    public static class PlcManagerFactory
    {
        public static IPlcManager Create(Machine machine)
        {
            switch (machine.MachineType)
            {
                case "BYMakinesi":
                    return new BYMakinesiManager(machine.IpAddress, machine.Port);

                case "Kurutma Makinesi":
                    return new KurutmaMakinesiManager(machine.IpAddress, machine.Port);

                default:
                    // Throw an exception for an unknown machine type to prevent the program from crashing.
                    throw new ArgumentException($"Unknown machine type: '{machine.MachineType}'. Please check machine settings.");
            }
        }
    }
}