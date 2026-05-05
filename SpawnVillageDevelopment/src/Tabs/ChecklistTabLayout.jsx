const TABLE_HEADERS = [
  'Note',
  'Done',
  'Content',
  'Task',
  'Cost',
  'Reward Res',
  'Reward XP',
  'CP Prod',
  'Pop',
  'Balance',
  'EXP',
  'CP',
  'Pop',
]

function buildNoteCellMeta(rows) {
  const meta = []

  for (let i = 0; i < rows.length; i += 1) {
    const note = rows[i].note.trim()

    if (note === '') {
      meta.push({ show: true, rowSpan: 1, sourceIndex: i })
      continue
    }

    let end = i + 1
    while (end < rows.length && rows[end].note.trim() === '') {
      end += 1
    }

    meta.push({ show: true, rowSpan: end - i, sourceIndex: i })
    for (let j = i + 1; j < end; j += 1) {
      meta.push({ show: false, rowSpan: 0, sourceIndex: i })
    }

    i = end - 1
  }

  return meta
}

export function ChecklistTable({ rows, onUpdateRow, hideNoteColumn }) {
  const noteCellMeta = buildNoteCellMeta(rows)
  const headers = hideNoteColumn ? TABLE_HEADERS.slice(1) : TABLE_HEADERS

  return (
    <div className="table-wrap">
      <table>
        <thead>
          <tr>
            {headers.map((header) => (
              <th key={header} scope="col">
                {header}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {rows.map((row, index) => (
            <tr key={row.id}>
              {!hideNoteColumn && noteCellMeta[index].show ? (
                <td rowSpan={noteCellMeta[index].rowSpan}>
                  <textarea
                    className="note-input"
                    rows={Math.max(2, noteCellMeta[index].rowSpan)}
                    value={rows[noteCellMeta[index].sourceIndex].note}
                    onChange={(event) =>
                      onUpdateRow(rows[noteCellMeta[index].sourceIndex].id, {
                        note: event.target.value,
                      })
                    }
                    placeholder=""
                    aria-label="Note"
                  />
                </td>
              ) : null}
              <td className="done-cell">
                <input
                  type="checkbox"
                  checked={row.done}
                  onChange={(event) => onUpdateRow(row.id, { done: event.target.checked })}
                  aria-label="Done"
                />
              </td>
              <td>{row.content}</td>
              <td>{row.task}</td>
              <td>{row.cost}</td>
              <td>{row.rewardRes}</td>
              <td>{row.rewardXp}</td>
              <td>{row.cpProd}</td>
              <td>{row.pop}</td>
              <td>{row.balance}</td>
              <td>{row.exp}</td>
              <td>{row.cp}</td>
              <td>{row.popTotal}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

function ChecklistTabLayout({ tabName, tribe, heroLevel, rows, onUpdateRow }) {
  return (
    <section className="tab-panel" aria-live="polite">
      <div className="panel-topbar">
        <div className="tab-meta">
          <h2>{tabName}</h2>          
        </div>
      </div>

      <ChecklistTable rows={rows} onUpdateRow={onUpdateRow} />
    </section>
  )
}

export default ChecklistTabLayout
