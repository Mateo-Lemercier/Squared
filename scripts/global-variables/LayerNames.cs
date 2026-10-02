
enum LayerNames : int {
    PlayerActive     = 1 << 0, // Layer 1
    PlayerInactive   = 1 << 1, // Layer 2

    Platform         = 1 << 4, // Layer 5
    PlatformActive   = 1 << 5, // Layer 6
    PlatformInactive = 1 << 6, // Layer 7

    Damage           = 1 << 7, // Layer 8
    DamageActive     = 1 << 8, // Layer 9
    DamageInactive   = 1 << 9, // Layer 10
}
