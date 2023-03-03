public enum Status {
    Unreal, Hand, Graveyard, Board, Hidden
}

public enum ElementalAffinity {
    Fire, Water, Electric, Earth, Combat, Mental, Neutral
}

public enum Type {
    Spell, Enchantment, Aura, CounterAttack, Echo, Sbire
}

public enum From {
    Monster, Equipment1, Equipment2, Equipment3, Equipment4
}

public enum TargetType {
    PlayerMonster, OpponantMonster, PlayerEquipment, PlayerCardEnchantment, OpponantCardEnchantment, PlayerAura, PlayerCardAura, OpponantCardAura, SlotVisible, SlotHidden, PlayerCardSbire, OpponantCardSbire
}

public enum BuffDebuffType {
    Power, Guard, Speed, Mana, DamageRaw, DamagePercent
}

// quickness = célérité / pierce = piétinement / lead combat = Initiative
public enum SbirePassifEffect {
    Quickness, Pierce, LeadCombat
}