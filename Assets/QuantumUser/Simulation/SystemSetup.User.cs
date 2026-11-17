namespace Quantum
{
    using System;
    using System.Collections.Generic;

    public static partial class DeterministicSystemSetup
    {
        static partial void AddSystemsUser(ICollection<SystemBase> systems, RuntimeConfig gameConfig, SimulationConfig simulationConfig, SystemsConfig systemsConfig)
        {
            // Tự động spawn nhân vật khi người chơi kết nối
            systems.Add(new PlayerSpawningSystem());
            
            // Xử lý di chuyển 8 hướng và dash
            systems.Add(new PlayerMovementSystem());
            
            // Xử lý đòn đánh combo và hồi sinh co-op
            systems.Add(new PlayerCombatSystem());
        }
    }
}
