# unity에셋을 활용한 간단한 TempleRun 게임

Bridge
- Bridge를 Prefab으로 만들어 랜덤하게 생성되게 하며 끝의 Brigde는 항상 Rotary로 설정.
- 이 게임의 가장 핵심적인 부분 = 다리는 한번에 10개씩 생성, 플레이어의 회전방향에 따라 다리의 위치가 달라져야함.
- 회전 방향에 따른 좌표계산이 핵심. 다리를 만든 후 newBridge의 하위 오브젝트로 설정할 것이므로 다리는 localPosition을 기준으로 만든다.
- 플레이어가 지니간 다리는 삭제하고 새로운 다리의 시작점을 만든다.
- Rotary의 Trigger를 설정하여 Player가 Trigger 안에 진입시 회전이 가능하게 Player의 상태를 회전 가능한 상태로 변경하여 회전한다.

Player
-Player는 Raycast를 활용하여 이동과 충돌을 검사
-회전각을 구하여 좌,우 회전

