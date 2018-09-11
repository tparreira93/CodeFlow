# Code Flow
[![Build status](https://ci.appveyor.com/api/projects/status/euofrovmpm2v7ecd?svg=true)](https://ci.appveyor.com/project/tparreira93/codeflow)

## Known issues

*   Nothing

## TODO

*   Margin indicator when code is submited to genio.
*   Help window for types of routine.
*   Save settings for search window.
*   Verification o db version in static version system.

## Versions

### 09/12/2017 - v2.1.0

*   It is now possible to remove manual code from Genio. Empty local manual code will show as manual code deletion in commit form.
*   Code that is in solution but not in genio database will now be shown as "Not found" in commit form.
*   Added option to log all commits.
*   Added possbility to compare, undo and redo commits.
*   Parallel search for manual code tags.
*   Refactored commit code. Operations are now defined by the type of difference. It gives more flexibility for future types of commit.
*   Refactored tag match in code. All types of manual code are now registered as ManualMatchProvider. This gives more flexibility for new manual code types.

### 07/12/2017 - v2.0.13

*   Bug fixes.
*   Sort data in code search.

### 05/12/2017 - v2.0.12

*   Bug fixes.

### 05/12/2017 - v2.0.11

*   Bug fixes.

### 05/12/2017 - v2.0.10

*   Fix in custom merge tool.

### 04/12/2017 - v2.0.9

*   Support for different merge tools.
*   Default line endings to DOS line endings.
*   Changed behaviour of commit form. It now commits only checked items.
*   Conflict window now only allows to merge with database.

### 29/11/2017 - v2.0.8

*   Updated icons.
*   Commit solution now reads files with correct encoding.
*   Automatic search for infoReindex.xml in all folders and subfolders of solution.
*   Fix in FNTX index for VCC++ solutions. It was removing end of line.
*   Fix in profile selection. If active profile was updated, changes would not persist until it was selected again.

### 29/11/2017 - v2.0.7

*   Added information about solution version and information about current genio profile to commit form.

### 24/11/2017 - v2.0.6

*   Fix in search window.
*   Commit form only loads differences.

### 24/11/2017 - v2.0.5

*   Fix in profiles form.
*   Fix in command Create.
*   Changed all icons to Microsof Visual Studio icons.
*   New search window that uses Microsoft Visual Studio controls and commands.
*   Commit, Update and Create can now be executed from context menu.
*   Automatic retrieval of Genio checkout path, System name, Genio versio and current database version.
*   Automatic parse of all types of routine.
*   Added auto commit to Tools->Options->Genio. This options is only available for manual code that was opened from search tool window.

### 24/11/2017 - v2.0.4

*   Fix in auto commit when manual code is opened from search tool window.

### 24/11/2017 - v2.0.3

*   Fix in backoffice indexes. It was doing convertion for ManualCode obtained from the database.
*   Fix in solution analysis. It now uses the correct enconding when comparing changes with database.
*   Fix in profiles form. It was creating an empty profile for every profile created
*   Fix in manual code extension when opened from search tool window.
*   Automatic commit to database when manual code is opened from search tool window.
*   Added new suggestions. ALT + ENTER to use.

## License
[Apache 2.0](LICENSE)
