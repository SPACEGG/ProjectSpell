namespace Spell.Model.Enums
{
    /// <summary>
    /// 주문에 사용할 수 있는 트레일 이펙트 프리팹 이름 Enum.
    /// Resources/VFX/TrailEffects 폴더 내 실제 프리팹 파일명(확장자 제외)과 일치해야 함.
    /// </summary>
    public enum TrailNameType
    {
        None,               // 트레일 없음 또는 기본값
        VFX_Trail_Earth,    // 흙/돌/진동 이펙트
        VFX_Trail_Fire,     // 불 속성
        VFX_Trail_Ice       // 얼음 속성
    }
}
