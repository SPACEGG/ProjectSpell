namespace Spell.Model.Enums
{
    /// <summary>
    /// 주문에 사용할 수 있는 머티리얼 이름 Enum.
    /// Resources/VFX/Materials 폴더 내 실제 머티리얼 파일명(확장자 제외)과 일치해야 함.
    /// </summary>
    public enum MaterialNameType
    {
        None,
        Earth, // 회색/흙/바위
        Fire,  // 불/용암/붉은
        Ice    // 얼음/푸른
    }
}
