using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public List<CharacterData> characterList;

    public EnemySpawner enemySpawner;
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;

    void Start()
    {
        int index = PlayerPrefs.GetInt("SelectedCharacter", 0);
        GameObject player = Instantiate(characterList[index].characterPrefab, Vector3.zero, Quaternion.identity);
        InitializeStartingSkills(characterList[index], player);
        PlayerHUD.instance.SetCharacterIcon(characterList[index].icon);
        virtualCamera.Follow = player.transform;
        virtualCamera.LookAt = player.transform;
        if (enemySpawner != null)
        {
            enemySpawner.Initialize(player.transform); // Gán player
            enemySpawner.StartSpawning();             // Bắt đầu spawn
        }
    }
    void InitializeStartingSkills(CharacterData characterData, GameObject player)
    {
        PlayerSkillManager skillManager = player.GetComponent<PlayerSkillManager>();
        if (skillManager != null)
        {
            // Tìm SkillData thực tế từ danh sách allSkills của PlayerExp
            // (Đảm bảo PlayerExp đã tồn tại và allSkills đã được gán trong Inspector)
            SkillData actualStartingSkillData = PlayerExp.Instance.allSkills.Find(
                s => s.skillType == characterData.startingSkill
            );

            if (actualStartingSkillData != null)
            {
                // Gọi EquipSkill để nó xử lý việc thêm skill và cập nhật số lượng slot
                // Hàm EquipSkill đã có logic kiểm tra và tăng currentAttackSkillCount/currentSupportSkillCount
                // Tuy nhiên, EquipSkill sẽ tăng currentLevel lên 1 nếu chưa có,
                // hoặc tăng thêm 1 level nếu đã có.
                // Nếu skill khởi đầu có level > 1, chúng ta cần một cách khác.

                // Cách tốt nhất là thêm một hàm mới vào PlayerSkillManager để chỉ định level
                // hoặc sửa đổi logic EquipSkill để nó có thể nhận level khởi tạo.

                // Để đơn giản và hiệu quả, chúng ta sẽ làm thế này:
                // Thêm skill vào equippedSkills và cập nhật count thủ công, sau đó apply effect.
                // Việc này cần cẩn thận để không trùng lặp logic với EquipSkill.

                // Phương án 1: Thêm một hàm mới vào PlayerSkillManager để "Pre-equip" skill
                // Đây là phương án sạch sẽ nhất.
                skillManager.PreEquipStartingSkill(actualStartingSkillData, characterData.startingSkillLevel);
            }
            else
            {
                Debug.LogError($"Không tìm thấy SkillData cho kỹ năng khởi đầu '{characterData.startingSkill}' trong danh sách allSkills của PlayerExp.");
            }
        }
    }
}
