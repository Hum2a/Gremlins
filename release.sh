#!/usr/bin/env bash

# Release tag manager for Gremlins (.NET / WPF)
# Creates semantic version tags, bumps Gremlins.csproj, installer/gremlins.iss, README.md;
# updates CHANGELOG.md for stable releases (no --name).

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR" || exit 1

# Initialize variables
INCREMENT=""
NAME=""
SET_TAG=""
SHOW_CURRENT=false
FORCE=false

CSPROJ="Gremlins.csproj"
ISS="installer/gremlins.iss"

show_help() {
  echo "Usage: $0 [OPTIONS]"
  echo "Manage release tags with semantic versioning for Gremlins."
  echo "Updates ${CSPROJ} (<Version>, assembly/file version), ${ISS}, README **App version:** and installer filenames;"
  echo "updates CHANGELOG.md for stable releases (no --name)."
  echo ""
  echo "Options:"
  echo "  --major           Increment major version (vX.0.0)"
  echo "  --minor           Increment minor version (v0.X.0)"
  echo "  --patch           Increment patch version (v0.0.X) (default)"
  echo "  --name NAME       Append custom name to version (e.g., beta)"
  echo "  --set-tag TAG     Set specific tag (must be vX.Y.Z format)"
  echo "  --current         Show current release tag"
  echo "  --force           Force tag creation even if commit is tagged"
  echo "  --help            Show this help message"
  echo ""
  echo "Examples:"
  echo "  $0 --current"
  echo "  $0 --minor"
  echo "  $0 --major --name beta"
  echo "  $0 --set-tag v1.2.3"
  exit 0
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --major)
      if [[ -n "$INCREMENT" ]]; then
        echo "Error: Cannot use multiple version flags together (--major, --minor, --patch, --set-tag)"
        exit 1
      fi
      INCREMENT="major"
      shift
      ;;
    --minor)
      if [[ -n "$INCREMENT" ]]; then
        echo "Error: Cannot use multiple version flags together (--major, --minor, --patch, --set-tag)"
        exit 1
      fi
      INCREMENT="minor"
      shift
      ;;
    --patch)
      if [[ -n "$INCREMENT" ]]; then
        echo "Error: Cannot use multiple version flags together (--major, --minor, --patch, --set-tag)"
        exit 1
      fi
      INCREMENT="patch"
      shift
      ;;
    --name)
      if [[ "$SHOW_CURRENT" == true ]]; then
        echo "Error: Cannot use --name with --current"
        exit 1
      fi
      NAME="$2"
      shift 2
      ;;
    --set-tag)
      if [[ -n "$INCREMENT" ]]; then
        echo "Error: Cannot use multiple version flags together (--major, --minor, --patch, --set-tag)"
        exit 1
      fi
      SET_TAG="$2"
      if [[ ! "$SET_TAG" =~ ^v[0-9]+\.[0-9]+\.[0-9]+(-[a-zA-Z0-9-]+)?$ ]]; then
        echo "Error: Tag must be in format vX.Y.Z or vX.Y.Z-NAME (e.g., v1.2.3 or v1.2.3-beta)"
        exit 1
      fi
      INCREMENT="set"
      shift 2
      ;;
    --current)
      if [[ -n "$INCREMENT" || -n "$NAME" || "$FORCE" == true ]]; then
        echo "Error: Cannot combine --current with other options"
        exit 1
      fi
      SHOW_CURRENT=true
      shift
      ;;
    --force)
      if [[ "$SHOW_CURRENT" == true ]]; then
        echo "Error: Cannot use --force with --current"
        exit 1
      fi
      FORCE=true
      shift
      ;;
    --help)
      show_help
      ;;
    *)
      echo "Error: Unknown option $1"
      show_help
      exit 1
      ;;
  esac
done

if [[ -z "$INCREMENT" && "$SHOW_CURRENT" == false ]]; then
  INCREMENT="patch"
fi

echo "Syncing with remote tags..."
git fetch --tags --force >/dev/null 2>&1

CURRENT_COMMIT=$(git rev-parse HEAD)

LATEST_TAG=$(git ls-remote --tags --refs --sort=-v:refname origin | head -n 1 | sed 's/.*\///')

if [[ "$SHOW_CURRENT" == true ]]; then
  if [[ -z "$LATEST_TAG" ]]; then
    echo "No releases found"
    exit 0
  fi

  TAG_COMMIT=$(git ls-remote origin refs/tags/"$LATEST_TAG" | cut -f 1)
  echo "Latest release tag: $LATEST_TAG"
  echo "Tag points to commit: $TAG_COMMIT"
  echo "Current commit: $CURRENT_COMMIT"

  if [[ "$TAG_COMMIT" == "$CURRENT_COMMIT" ]]; then
    echo "Status: Current commit is tagged"
  else
    echo "Status: Current commit is not tagged"
  fi
  exit 0
fi

if [[ "$INCREMENT" == "set" ]]; then
  NEW_TAG="$SET_TAG"
  echo "Setting tag directly to: $NEW_TAG"
else
  if [[ -z "$LATEST_TAG" ]]; then
    LATEST_TAG="v0.0.0"
    echo "No existing tags found. Starting from v0.0.0"
  else
    echo "Current release tag: $LATEST_TAG"
  fi

  CLEAN_TAG=${LATEST_TAG#v}
  MAJOR=$(echo "$CLEAN_TAG" | cut -d. -f1)
  MINOR=$(echo "$CLEAN_TAG" | cut -d. -f2)
  PATCH=$(echo "$CLEAN_TAG" | cut -d. -f3 | sed 's/-.*//')

  case $INCREMENT in
    major)
      MAJOR=$((MAJOR + 1))
      MINOR=0
      PATCH=0
      ;;
    minor)
      MINOR=$((MINOR + 1))
      PATCH=0
      ;;
    patch)
      PATCH=$((PATCH + 1))
      ;;
  esac

  NEW_TAG="v${MAJOR}.${MINOR}.${PATCH}"

  if [[ -n "$NAME" ]]; then
    SANITIZED_NAME=$(echo "$NAME" | tr -cd '[:alnum:]-' | tr ' ' '-')
    NEW_TAG="${NEW_TAG}-${SANITIZED_NAME}"
  fi
fi

echo "Checking for existing tags..."
EXISTING_REMOTE=$(git ls-remote origin "refs/tags/${NEW_TAG}")
EXISTING_LOCAL=$(git tag -l "$NEW_TAG")

if [[ -n "$EXISTING_REMOTE" || -n "$EXISTING_LOCAL" ]]; then
  echo "Tag $NEW_TAG already exists - deleting old version"

  if [[ -n "$EXISTING_REMOTE" ]]; then
    echo "Deleting remote tag: $NEW_TAG"
    git push --delete origin "$NEW_TAG" >/dev/null 2>&1 || true
  fi

  if [[ -n "$EXISTING_LOCAL" ]]; then
    echo "Deleting local tag: $NEW_TAG"
    git tag -d "$NEW_TAG" >/dev/null 2>&1 || true
  fi
fi

if [[ -n "$LATEST_TAG" ]]; then
  TAG_COMMIT=$(git ls-remote origin refs/tags/"$LATEST_TAG" | cut -f 1)
  if [[ "$TAG_COMMIT" == "$CURRENT_COMMIT" && "$FORCE" == false ]]; then
    echo "Error: Current commit is already tagged as $LATEST_TAG"
    echo "Use --force to create a new tag on this commit"
    exit 1
  fi
fi

generate_changelog_from_commits() {
  local since_tag="$1"
  local added=""
  local changed=""
  local fixed=""
  local security=""
  local deprecated=""
  local removed=""
  local other=""

  local commit_range=""
  if [[ -z "$since_tag" || "$since_tag" == "v0.0.0" ]]; then
    commit_range="HEAD"
  else
    commit_range="${since_tag}..HEAD"
  fi

  local commits
  commits=$(git log "$commit_range" --pretty=format:"%s" --no-merges 2>/dev/null | grep -vE "^chore:.*(version|changelog|bump)" || true)
  commits=$(echo "$commits" | grep -v "^$" || true)

  if [[ -z "$commits" ]]; then
    echo ""
    return
  fi

  local temp_commits_file
  if command -v mktemp >/dev/null 2>&1; then
    temp_commits_file=$(mktemp)
  else
    temp_commits_file="/tmp/release_commits_$$.txt"
  fi
  echo "$commits" > "$temp_commits_file"

  while IFS= read -r commit_msg || [[ -n "$commit_msg" ]]; do
    [[ -z "$commit_msg" ]] && continue

    local type=""
    local description=""

    if echo "$commit_msg" | grep -Eq '^[a-z]+(\([^)]+\))?[[:space:]]*:'; then
      type=$(echo "$commit_msg" | sed -E 's/^([a-z]+)(\([^)]+\))?[[:space:]]*:.*/\1/')
      description=$(echo "$commit_msg" | sed -E 's/^[a-z]+(\([^)]+\))?[[:space:]]*:[[:space:]]*//' | sed 's/^[[:space:]]*//;s/[[:space:]]*$//')
    else
      description="$commit_msg"
      type="other"
    fi

    [[ -z "$description" ]] && continue

    if [[ -n "$description" ]]; then
      description=$(echo "$description" | sed 's/^./\U&/')
      if [[ ! "$description" =~ [.!?]$ ]]; then
        description="${description}."
      fi
    fi

    case "$type" in
      feat|feature)
        if [[ -z "$added" ]]; then
          added="- ${description}"
        else
          added="${added}
- ${description}"
        fi
        ;;
      fix|bugfix)
        if [[ -z "$fixed" ]]; then
          fixed="- ${description}"
        else
          fixed="${fixed}
- ${description}"
        fi
        ;;
      security)
        if [[ -z "$security" ]]; then
          security="- ${description}"
        else
          security="${security}
- ${description}"
        fi
        ;;
      deprecate|deprecated)
        if [[ -z "$deprecated" ]]; then
          deprecated="- ${description}"
        else
          deprecated="${deprecated}
- ${description}"
        fi
        ;;
      remove|removed)
        if [[ -z "$removed" ]]; then
          removed="- ${description}"
        else
          removed="${removed}
- ${description}"
        fi
        ;;
      change|changed|update|updated|refactor|perf|performance|style)
        if [[ -z "$changed" ]]; then
          changed="- ${description}"
        else
          changed="${changed}
- ${description}"
        fi
        ;;
      *)
        if [[ "$type" == "refactor" || "$type" == "perf" || "$type" == "style" ]]; then
          if [[ -z "$changed" ]]; then
            changed="- ${description}"
          else
            changed="${changed}
- ${description}"
          fi
        fi
        ;;
    esac
  done < "$temp_commits_file"

  rm -f "$temp_commits_file"

  local changelog_content=""

  if [[ -n "$added" ]]; then
    changelog_content="${changelog_content}
### Added
${added}"
  fi

  if [[ -n "$changed" ]]; then
    changelog_content="${changelog_content}
### Changed
${changed}"
  fi

  if [[ -n "$fixed" ]]; then
    changelog_content="${changelog_content}
### Fixed
${fixed}"
  fi

  if [[ -n "$security" ]]; then
    changelog_content="${changelog_content}
### Security
${security}"
  fi

  if [[ -n "$deprecated" ]]; then
    changelog_content="${changelog_content}
### Deprecated
${deprecated}"
  fi

  if [[ -n "$removed" ]]; then
    changelog_content="${changelog_content}
### Removed
${removed}"
  fi

  if [[ -z "$changelog_content" ]]; then
    changelog_content="
### Changed
- Various updates and improvements."
  fi

  echo "$changelog_content"
}

# Semver without leading v (e.g. 1.2.3 or 1.2.3-beta)
VERSION_SEMVER=${NEW_TAG#v}

# MAJOR.MINOR.PATCH.0 for AssemblyVersion / FileVersion (no prerelease suffix)
VERSION_CORE=$(echo "$VERSION_SEMVER" | sed 's/-.*//')
ASM_MAJOR=$(echo "$VERSION_CORE" | cut -d. -f1)
ASM_MINOR=$(echo "$VERSION_CORE" | cut -d. -f2)
ASM_PATCH=$(echo "$VERSION_CORE" | cut -d. -f3)
ASSEMBLY_FOUR_PART="${ASM_MAJOR}.${ASM_MINOR}.${ASM_PATCH}.0"

update_csproj_version() {
  [[ -f "$CSPROJ" ]] || { echo "Error: $CSPROJ not found in $(pwd)"; exit 1; }
  sed \
    -e "s|<Version>[^<]*</Version>|<Version>${VERSION_SEMVER}</Version>|" \
    -e "s|<AssemblyVersion>[^<]*</AssemblyVersion>|<AssemblyVersion>${ASSEMBLY_FOUR_PART}</AssemblyVersion>|" \
    -e "s|<FileVersion>[^<]*</FileVersion>|<FileVersion>${ASSEMBLY_FOUR_PART}</FileVersion>|" \
    "$CSPROJ" > "${CSPROJ}.tmp" && mv "${CSPROJ}.tmp" "$CSPROJ"
}

update_inno_setup() {
  [[ -f "$ISS" ]] || return 0
  sed \
    -e "s|^#define MyAppVersion \".*\"|#define MyAppVersion \"${VERSION_SEMVER}\"|" \
    -e "s|^OutputBaseFilename=Gremlins-Setup-.*|OutputBaseFilename=Gremlins-Setup-${VERSION_SEMVER}|" \
    "$ISS" > "${ISS}.tmp" && mv "${ISS}.tmp" "$ISS"
}

update_readme_versions() {
  [[ -f README.md ]] || return 0
  sed -E \
    -e "s|(\*\*App version:\*\* \`)[^\`]+(\`)|\1${VERSION_SEMVER}\2|" \
    -e "s|Gremlins-Setup-[0-9]+\.[0-9]+\.[0-9]+(-[a-zA-Z0-9-]+)?|Gremlins-Setup-${VERSION_SEMVER}|g" \
    README.md > README.md.tmp && mv README.md.tmp README.md
}

echo "Updating ${CSPROJ}, ${ISS}, README.md to version ${VERSION_SEMVER}..."
update_csproj_version
update_inno_setup
update_readme_versions

CHANGELOG_FILE="CHANGELOG.md"
CHANGELOG_UPDATED=false
if [[ -z "$NAME" && -f "$CHANGELOG_FILE" ]]; then
  echo "Updating CHANGELOG.md..."

  RELEASE_DATE=$(date -u +"%Y-%m-%d")
  VERSION_NUMBER=${VERSION_SEMVER}

  echo "Generating changelog entries from git commits since ${LATEST_TAG:-beginning}..."
  COMMIT_CHANGES=$(generate_changelog_from_commits "$LATEST_TAG")

  if command -v mktemp >/dev/null 2>&1; then
    TEMP_CHANGELOG=$(mktemp)
    TEMP_COMMITS=$(mktemp)
  else
    TEMP_CHANGELOG="${CHANGELOG_FILE}.tmp"
    TEMP_COMMITS="${CHANGELOG_FILE}.commits.tmp"
  fi

  echo "$COMMIT_CHANGES" > "$TEMP_COMMITS"

  awk -v version="$VERSION_NUMBER" -v date="$RELEASE_DATE" '
    BEGIN {
      in_unreleased = 0
      unreleased_content = ""
      version_inserted = 0
      has_commit_changes = 0

      commit_changes_file = "'"$TEMP_COMMITS"'"
      if ((getline commit_changes_line < commit_changes_file) > 0) {
        commit_changes = commit_changes_line
        while ((getline commit_changes_line < commit_changes_file) > 0) {
          commit_changes = commit_changes "\n" commit_changes_line
        }
        close(commit_changes_file)
        if (commit_changes != "" && commit_changes != "\n") {
          has_commit_changes = 1
        }
      }
    }

    /^## \[Unreleased\]/ {
      print
      in_unreleased = 1
      unreleased_content = ""
      next
    }

    /^## \[/ {
      if (in_unreleased && !version_inserted) {
        print ""
        printf "## [%s] — %s\n", version, date
        if (has_commit_changes) {
          print commit_changes
        } else if (unreleased_content != "") {
          print ""
          print unreleased_content
        }
        version_inserted = 1
      }
      in_unreleased = 0
      print
      next
    }

    in_unreleased {
      if (!has_commit_changes) {
        if (unreleased_content == "") {
          unreleased_content = $0
        } else {
          unreleased_content = unreleased_content "\n" $0
        }
      }
      next
    }

    {
      print
    }

    END {
      if (in_unreleased && !version_inserted) {
        print ""
        printf "## [%s] — %s\n", version, date
        if (has_commit_changes) {
          print commit_changes
        } else if (unreleased_content != "") {
          print ""
          print unreleased_content
        }
      }
    }
  ' "$CHANGELOG_FILE" > "$TEMP_CHANGELOG"

  rm -f "$TEMP_COMMITS"

  mv "$TEMP_CHANGELOG" "$CHANGELOG_FILE"
  CHANGELOG_UPDATED=true
fi

[[ "$CHANGELOG_UPDATED" == true ]] && git add "$CHANGELOG_FILE"
[[ -f "$CSPROJ" ]] && git add "$CSPROJ"
[[ -f "$ISS" ]] && git add "$ISS"
[[ -f README.md ]] && git add README.md

if ! git diff --cached --quiet; then
  if [[ "$CHANGELOG_UPDATED" == true ]]; then
    COMMIT_MSG="chore: update CHANGELOG and bump version for ${NEW_TAG}"
    echo "Committing CHANGELOG, ${CSPROJ}, installer, and README for version ${VERSION_SEMVER}..."
  else
    COMMIT_MSG="chore: bump version to ${VERSION_SEMVER} (${NEW_TAG})"
    echo "Committing ${CSPROJ}, installer, and README for version ${VERSION_SEMVER}..."
  fi
  git commit -m "$COMMIT_MSG" >/dev/null 2>&1
  git push origin HEAD >/dev/null 2>&1 || true
else
  echo "No file changes to commit (version files and CHANGELOG already match)"
fi

echo "Creating new tag: $NEW_TAG"
if git tag "$NEW_TAG" && git push origin "$NEW_TAG"; then
  echo "Successfully created release tag: $NEW_TAG"
  REMOTE_PATH=$(git remote get-url origin | sed -E 's/.*[:/]([^/]+\/[^/]+)\.git.*/\1/')
  echo "Tag URL: https://github.com/${REMOTE_PATH}/releases/tag/$NEW_TAG"
else
  echo "Error: Failed to create or push tag"
  exit 1
fi
