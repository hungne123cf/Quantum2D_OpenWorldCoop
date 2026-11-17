---
name: unity-quantum3-2d-coop
description: Quy tắc và quy trình chuẩn cho dự án game 2D top-down co-op 4 người, thế giới mở đi farm, làm bằng Unity 6000.4.8f1 + Photon Quantum 3 (deterministic rollback). Áp dụng mọi khi làm việc trong D:\Unity\Quantum2D_OpenWorldCoop hoặc bất kỳ tác vụ nào về Quantum 3 simulation, sprite sheet nhân vật chibi, VFX, farm tài nguyên, quái AI, hồi sinh co-op, map Tilemap.
---

# Unity 6 + Photon Quantum 3 — 2D Co-op Open World

## Thông tin dự án
- Đường dẫn: `D:\Unity\Quantum2D_OpenWorldCoop`
- Editor: `D:\Unity\6000.4.8f1\Editor\Unity.exe` (Unity 6000.4.8f1, URP 2D)
- GitHub: https://github.com/hungne123cf/Quantum2D_OpenWorldCoop (nhánh `main`, ảnh dùng Git LFS)
- Người chơi: 4 người co-op online, góc nhìn top-down 2.5D, chibi anime.
- Art: user tự vẽ bằng ChatGPT IDE. Sprite sheet mẫu ở `Assets/_Project/Art/Sprites/Characters/`
  (Swordmaster = cyan, Mage = magenta). Bố cục mỗi sheet: 6 hàng IDLE / RUN / ATTACK / HURT / DASH / DIE
  + panel phải ATTACK/DASH/HIT EFFECTS + PALETTE. Map sẽ làm sau.

## Quy tắc VÀNG của Quantum 3 (không bao giờ vi phạm)
1. **Simulation tách khỏi View.** Mọi gameplay (di chuyển, combat, sát thương, máu, farm, rơi đồ, quái AI,
   hồi sinh, inventory) nằm trong Quantum simulation. Unity MonoBehaviour chỉ hiển thị (sprite, VFX, âm thanh, UI, camera).
2. **Chỉ dùng Fixed Point** trong simulation: `FP`, `FPVector2`, `FPMath`. Cấm `float`, `double`, `Vector2`,
   `Mathf`, `UnityEngine.Random`, `Time.deltaTime`, `DateTime`, `Dictionary` duyệt không thứ tự.
   Random dùng `frame.RNG`, thời gian dùng `frame.DeltaTime`.
3. **State nằm trong component (DSL `.qdef`)**, system là stateless. Không lưu biến tĩnh/field trong system.
4. **Input chỉ qua `input {}` DSL** (MoveDirection, AimDirection, Attack, Dash, SkillA, SkillB, Interact).
   Unity gửi input trong callback `PollInput`.
5. **View nhận dữ liệu qua** `EntityView` / `QuantumEntityViewUpdater` và **Events** (`frame.Events.X(...)`)
   — dùng cho VFX, âm thanh. Event là non-deterministic phía view, không được ảnh hưởng gameplay.
6. Physics dùng **Quantum Physics2D** (`PhysicsCollider2D`, `PhysicsBody2D`, `frame.Physics2D.OverlapShape`),
   không dùng `Rigidbody2D`/`Collider2D` của Unity cho logic.
7. Asset cấu hình (chỉ số class, quái, tài nguyên) dùng `AssetObject` của Quantum, tham chiếu qua `AssetRef`.

## Cấu trúc thư mục
- `Assets/QuantumUser/Simulation/` — `.qdef` + system C# (đặt ở đây sau khi import SDK để codegen nhận).
  Các file nháp hiện ở `Assets/_Project/Scripts/Quantum/` cần di chuyển vào đây.
- `Assets/_Project/Scripts/UnityView/` — animator, VFX, camera, UI.
- `Assets/_Project/Scripts/Editor/` — tool editor (vd. `SpriteSheetAutoSlicerWindow`).
- `Assets/_Project/Art/` — sprite, VFX, tileset.

## Trạng thái hiện tại (cập nhật khi tiến triển)
- [x] Khung dự án, sprite, push GitHub.
- [ ] Import Photon Quantum 3 SDK + App ID (user làm).
- [ ] Viết lại `PlayerSimulationCore.cs` (đang dùng float — CHƯA deterministic) thành system FP.
- [ ] `WorldResourceNode.cs` đang dùng Random/Coroutine phía Unity — phải chuyển logic vào simulation.
- [ ] Slice sprite sheet, tạo clip animation.

## Gameplay chuẩn
- Di chuyển 8 hướng, Dash có i-frame ~0.25s + cooldown, combo đánh 3 hit (reset nếu ngừng bấm).
- 4 class: Swordmaster (melee), Mage (AOE), Ranger (tầm xa), Paladin (tank) — 2 class sau chờ art.
- Co-op: HP về 0 -> Downed; đồng đội giữ Interact 3s gần đó để hồi sinh; cả đội Downed -> về checkpoint.
- Farm: cây (gỗ), quặng (sắt/pha lê), thảo dược; node có durability + respawn timer, rơi item trong simulation.
- Quái: state machine Idle/Chase/Attack/Hurt/Dead, chọn mục tiêu player gần nhất trong DetectRadius.

## Quy trình làm việc
1. Đổi gameplay -> sửa `.qdef` trước, chạy Quantum codegen, rồi viết system.
2. Đăng ký system mới vào `SystemsConfig`.
3. Kiểm tra compile bằng Unity batchmode, đọc log:
   `& "D:\Unity\6000.4.8f1\Editor\Unity.exe" -batchmode -quit -projectPath "D:\Unity\Quantum2D_OpenWorldCoop" -logFile "D:\Unity\quantum2d_compile.log"`
   (đóng Unity Editor đang mở project trước khi chạy batchmode).
4. Test multiplayer bằng Quantum local 4 player / nhiều client build trước khi commit.
5. Commit thông điệp rõ ràng, push `origin main`. Không commit `Library/`, `Temp/`, `Logs/`, `UserSettings/`.

## Sprite & art
- Import: Sprite (2D and UI), Multiple, pivot bottom-center (0.5, 0.1), Uncompressed, Filter Bilinear.
- Sheet nền đen (Mage .jpg) cần tách nền/alpha trước khi dùng; ưu tiên yêu cầu user xuất PNG nền trong suốt.
- Prompt ChatGPT mẫu: giữ nguyên bố cục 6 hàng + panel VFX + palette, chibi top-down 2.5D, nền trong suốt.

## Tuyệt đối không
- Không viết gameplay trong MonoBehaviour rồi gọi là "đã sync".
- Không tuyên bố đã build/chạy được nếu chưa compile thật và đọc log.
- Không xoá dữ liệu / force push khi chưa hỏi user.
