namespace Spell.Model.Enums
{
    /// <summary>
    /// 주문에 사용할 수 있는 파티클 프리팹 이름 Enum.
    /// Resources/VFX/Particles 폴더 내 실제 프리팹 파일명(확장자 제외)과 일치해야 함.
    /// </summary>
    public enum ParticleNameType
    {
        None, // 파티클 없음 또는 기본값

        Ef_IceMagicGlowFree01,             // 얼음 계열의 마법 글로우 효과, 푸른빛이 도는 기본적인 얼음 마법 오라
        prefV_vfx_fire_yellow,             // 노란 계열 불꽃 이펙트
        prefV_vfx_fire_blue,               // 푸른 불꽃 (보통 차가운 느낌의 마법 불꽃)
        prefV_vfx_fire_darkBlue,           // 더 어두운 톤의 파란 불꽃, 강력하거나 심오한 마법 느낌
        prefV_vfx_fire_electric_blue,      // 파란 전기 불꽃, 감전 효과 연출 가능
        prefV_vfx_fire_electric_darkBlue,  // 어두운 파란색 전기 이펙트
        prefV_vfx_fire_electric_green,     // 초록 전기 불꽃, 독성/에너지 느낌
        prefV_vfx_fire_electric_orange,    // 주황 전기 불꽃, 화염 + 전기 혼합형
        prefV_vfx_fire_electric_pink,      // 핑크 전기 불꽃, 마법적 또는 여성향 연출
        prefV_vfx_fire_electric_purple,    // 보라 전기 이펙트, 암흑/마법 공격 연출용
        prefV_vfx_fire_electric_red,       // 빨간 전기 불꽃, 파괴적 느낌
        prefV_vfx_fire_electric_white,     // 하얀 전기 불꽃, 신성하거나 강력한 기술 느낌
        prefV_vfx_fire_electric_yellow,    // 노란 전기 불꽃, 감전, 스파크 효과
        prefV_vfx_fire_green,              // 초록 불꽃, 독성/자연 마법 느낌
        prefV_vfx_fire_orange,             // 주황 불꽃, 일반적인 불 이펙트
        prefV_vfx_fire_pink,               // 핑크 불꽃, 판타지한 마법 이펙트용
        prefV_vfx_fire_purple,             // 보라색 불꽃, 마법/암흑 속성 느낌
        prefV_vfx_fire_red,                // 빨간 불꽃, 기본 파괴계 마법
        prefV_vfx_fire_snow_blue,          // 파란 불꽃 + 눈 이펙트 혼합, 냉기 마법
        prefV_vfx_fire_snow_darkBlue,      // 어두운 파란색 냉기 불꽃
        prefV_vfx_fire_snow_green,         // 초록 냉기 불꽃, 독기+얼음 혼합 느낌
        prefV_vfx_fire_snow_orange,        // 주황 불꽃+눈 조합, 이색적인 효과
        prefV_vfx_fire_snow_pink,          // 핑크 불꽃+눈, 여성향 판타지 효과
        prefV_vfx_fire_snow_purple,        // 보라 불꽃+눈, 암흑+냉기 느낌
        prefV_vfx_fire_snow_red,           // 붉은 불꽃+눈, 강력한 냉염 혼합
        prefV_vfx_fire_snow_white,         // 하얀 불꽃+눈, 신성 or 빙결 마법
        prefV_vfx_fire_snow_yellow,        // 노란 불꽃+눈, 빛 속성 또는 특이 마법
        prefV_vfx_fire_tech_blue,          // 파란 불꽃 + SF 느낌의 tech 효과
        prefV_vfx_fire_tech_darkBlue,      // 어두운 파란 tech 불꽃
        prefV_vfx_fire_tech_green,         // 초록 tech 불꽃, 에너지 무기 이펙트
        prefV_vfx_fire_tech_orange,        // 주황 tech 불꽃, 사이버 불꽃 느낌
        prefV_vfx_fire_tech_pink,          // 핑크 tech 불꽃, SF 마법 효과
        prefV_vfx_fire_tech_purple,        // 보라 tech 불꽃, 사이킥/마법 기술 연출
        prefV_vfx_fire_tech_red,           // 빨간 tech 불꽃, 공격 기술 연출
        prefV_vfx_fire_tech_white,         // 하얀 tech 불꽃, 고대 또는 신성 기술 느낌
        prefV_vfx_fire_tech_yellow,        // 노란 tech 불꽃, 경고등/에너지 느낌
        prefV_vfx_fire_white,              // 하얀 불꽃, 신성 마법 또는 특수 화염
        Sparks_green,                      // 초록색 불꽃 스파크, 자연 or 중독 이펙트
        Sparks_pink,                       // 핑크빛 스파크, 마법/요정 느낌
        Sparks_red,                        // 빨간 스파크, 작은 폭발 or 불씨 연출
        Sparks_white,                      // 흰색 스파크, 전기 or 신성 효과
        Sparks_yellow,                     // 노란 스파크, 전기 or 경고 효과
        vfx_fire_colorChange               // 불꽃 색상이 변하는 이펙트, 색상 시프트 연출용
    }
}
