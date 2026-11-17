namespace Quantum
{
    using System;
    using System.Collections.Generic;

    public static partial class DeterministicSystemSetup
    {
        static partial void AddSystemsUser(ICollection<SystemBase> systems, RuntimeConfig gameConfig, SimulationConfig simulationConfig, SystemsConfig systemsConfig)
        {
            // Đăng ký các Simulation Systems chính cho game 2D Co-op
            systems.Add(new PlayerMovementSystem());
            systems.Add(new PlayerCombatSystem());
        }
    }
}
