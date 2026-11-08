# REA — مساحة إعادة بناء الألعاب (Clean-Room)

مساحة عمل لعبة الصيد والنجاة: هندسة عكسية للمرجع (Muck / HowToFish) في `artifacts/` فقط للدراسة،
وإعادة بناء أصلية بالكامل من الصفر — لا يُنسخ كود أو أصول من أي لعبة.

## ما داخل المساحة

| المشروع | النوع | الحالة |
|---|---|---|
| `WebGame/` | **Tidebreak** — لعبة صيد ونجاة في المتصفح (Three.js) | ✅ شغالة ومُحدثة — أسلحة جديدة + خادم وطبقة Multiplayer (Socket.io) — بوابة `npm run verify` تعدّ 54/54 |
| `UnityProject/` (Muck Track) | كلون Muck بـ Unity 6 + FishNet (60+ سكربت) | ✅ الكود كامل ومُطور — إضافة نظام الستياما الكامل (Stamina Bar + Consumption)، وترسانة أسلحة ودروع جديدة (Iron Sword, War Hammer, Bone Bow, Obsidian Katana, Full Iron Armor) |
| `UnityProject/` (FPS Track) | محرك تصويب وحركة كينتيك عالية السرعة (Combat Master Style) | ✅ محرك متكامل — حركة كينتيك (سلايد + جمب باد + زيب لاين) + ترسانة 4 أسلحة (AR, Shotgun, SMG, Sniper) + WeaponManager ومبني المشهد `Create FPS Arena Scene` |

## Tidebreak (الويب)

- التشغيل: افتح `WebGame/dist/tidebreak.html` مباشرة، أو من سيرفر المعاينة المحلي.
- خادم اللعب الجماعي: تشغيل `node WebGame/server/index.js` لمشاركة حركة اللاعبين لحظيًا.
- الترسانة الجديدة: Tide Harpoon, Signal Flare Gun, Shock Spear, Abyssal Rod, Glowing Squid, Golden Mahi.
- المحرك: محيط إجرائي بأمواج، جزر من noise، قارب بفيزياء، دورة صيد كاملة، مخزون 24 خانة،
  جورنال بالبحث، ودوكهاند بيرد عبر موديل لغوي (بدون أي API key داخل الحزمة).
- البوابة: `npm run verify` = اختبارات + بناء ملف واحد + تشغيل 300 إطار فعلي. تفشل لو أي
  موديل ناقص أو الحزمة فيها سر أو CDN خارجي.

## كلون Unity (Muck Track)

- اللوب الكامل مكتوب ومُطور: حركة، قتال، مخزون، كرافت، صندوق مشترك شبكي، عملة، حفظ تلقائي،
  دو-نايت وموجات أعداء، حصاد خشب/حجر/ليف، ملابس ودروع، بوس يسقط Boss Key.
- نظام الستياما (Stamina): تم ربط الجري والقفز باستهلاك الستياما، مع شريط أخضر تفاعلي في الـ HUD وتجدد تلقائي.
- الأسلحة والكرافت: إضافة سيف حديدي، مطرقة حربية، قوس عظام مع أسهم، دروع حديدية كاملة، وكاتانا أوبسيديان.
- الإنشاء الفعلي للعبة: افتح المشروع في Unity 6000.0.32f1 → منيو **Clone → Scene →
  Create Main Scene** (يبني المشهد والمحمولات والمواد ويوصل الـ BossDirector تلقائيًا) →
  Play → اضغط **Host (Server + Client)**.
- الواجهات (الصندوق [E]، الكرافت [C]، HUD اللاعب) تُبنى runtime — لا تحتاج ربط مشهد.

## محرك وتجربة الـ FPS (مسار السرعة العالية)

- كود متكامل في `Assets/Scripts/Fps/` ومبني مشهد في `Assets/Editor/FpsArenaBuilder.cs`.
- الترسانة: Assault Rifle, Tactical Shotgun (8 pellets), Rapid SMG (16 rps), Precision Bolt Sniper (zoom ADS).
- إدارة الأسلحة: `FpsWeaponManager` للتبديل بين الأسلحة بالمفاتيح [1-4] أو بكرة الفأرة مع واجهة HUD حية للذخيرة.
- إنشاء ساحة الـ FPS: منيو **Clone → Scene → Create FPS Arena Scene** يبني تلقائيًا ساحة الـ 3-Lanes مع غرف الـ CQB، أبراج القناصة، منصات الإطلاق، الزيب لاين، وأهداف التدريب المتفاعلة.

## أدوات الدراسة

`tools/` فيها أدوات استخراج الـ IL والـ strings من نسخة الدراسة، و`artifacts/` فيها ملاحظات
التحليل (ميكانيكا الصيد، الجورنال، الصناديق، الحفظ) — كلها مرجع سلوكي فقط.

> مسار TypeScript/Electron الموثق في `CLONE_PLAN-TS.md` ليس لعبة — هو إعادة بناء لأداة REA نفسها.
