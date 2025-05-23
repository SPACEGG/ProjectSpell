namespace Spell.Model.Enums
{
    /// <summary>
    /// 주문에 사용할 수 있는 파티클 프리팹 이름 Enum.
    /// Resources/VFX/Particles 폴더 내 실제 프리팹 파일명(확장자 제외)과 일치해야 함.
    /// </summary>
    public enum ParticleNameType
    {
        None,       // 파티클 없음 또는 기본값
        Sparks_red  // 빨간 스파크, 작은 폭발 or 불씨 연출
    }
}
