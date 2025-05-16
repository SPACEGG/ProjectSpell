namespace Spell.Model.Enums
{
    /// <summary>
    /// 주문에 사용할 수 있는 트레일 이펙트 프리팹 이름 Enum.
    /// Resources/VFX/TrailEffects 폴더 내 실제 프리팹 파일명(확장자 제외)과 일치해야 함.
    /// </summary>
    public enum TrailNameType
    {
        None,                   // 트레일 없음 또는 기본값

        VFX_Trail_Cosmos,       // 우주 느낌의 트레일. 별, 별가루, 갤럭시 같은 환상적이고 몽환적인 궤적 가능성
        VFX_Trail_Dark,         // 어둠 속성. 검붉은 연기, 암흑 에너지, 혹은 그림자 같은 꼬리 효과일 확률
        VFX_Trail_Earth,        // 흙/돌/진동 이펙트. 갈색 또는 돌 부스러기처럼 무거운 궤적 연출 예상
        VFX_Trail_Electric,     // 번개 속성. 번쩍이는 스파크, 노란/파란 번개 줄기 같은 날카로운 에너지 자취
        VFX_Trail_Fire,         // 불 속성. 주황색/붉은 불꽃이 따라붙는 열기 강한 궤적
        VFX_Trail_Ice,          // 얼음 속성. 파란색 결정 파편, 서리 입자처럼 차가운 자취
        VFX_Trail_Nature,       // 자연 속성. 이끼, 풀 입자, 꽃잎처럼 생명감 있는 트레일일 가능성
        VFX_Trail_Sound,        // 소리 시각화 이펙트. 웨이브, 리듬 파형, 진동선이 움직임에 따라 퍼지는 느낌
        VFX_Trail_Void,         // 공허/차원 속성. 퍼플+블랙의 왜곡된 흔적, 소용돌이 효과일 가능성
        VFX_Trail_Water         // 물 속성. 물결무늬, 물방울 스플래시, 반짝이는 청량한 잔상 형태일 가능성
    }
}
