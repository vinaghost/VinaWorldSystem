import { Fragment, useEffect, useMemo, useState } from 'react'
import { fourPartyFarmData, fourPartyFarmNote } from './data/4p-farm'
import { fourPartySimData, fourPartySimNote } from './data/4p-sim'
import { threePartyFarmData, threePartyFarmNote } from './data/3p-farm'
import { threePartySimData, threePartySimNote } from './data/3p-sim'
import PWABadge from './PWABadge.jsx'
import './App.css'

const TABS = [
  { id: '4p-sim', label: '4 Party - Sim', rows: fourPartySimData, notes: fourPartySimNote },
  { id: '4p-farm', label: '4 Party - Farm', rows: fourPartyFarmData, notes: fourPartyFarmNote },
  { id: '3p-sim', label: '3 Party - Sim', rows: threePartySimData, notes: threePartySimNote },
  { id: '3p-farm', label: '3 Party - Farm', rows: threePartyFarmData, notes: threePartyFarmNote },
]

const INITIALS = {
  balance: 3000,
  exp: 0,
  cp: 0,
  population: 0,
}

function getNumber(value) {
  return Number(value) || 0
}

function formatNumber(value) {
  return getNumber(value).toLocaleString()
}

function buildComputedRows(rows) {
  let balance = INITIALS.balance
  let previousRewardRes = 0
  let exp = INITIALS.exp
  let cp = INITIALS.cp
  let population = INITIALS.population

  return rows.map((row, index) => {
    balance += previousRewardRes - getNumber(row.cost)
    const displayBalance = balance

    previousRewardRes = getNumber(row['reward res'])
    exp += getNumber(row['reward exp'])
    cp += getNumber(row['cp prod'])
    population += getNumber(row.pop)

    return {
      index,
      ...row,
      balance: displayBalance,
      exp,
      cp,
      population,
    }
  })
}

function App() {
  const [activeTab, setActiveTab] = useState(TABS[0].id)
  const [checkedByTab, setCheckedByTab] = useState({})

  useEffect(() => {
    const loaded = {}

    TABS.forEach((tab) => {
      try {
        const saved = window.localStorage.getItem(`spawn-checklist:${tab.id}`)
        if (!saved) {
          loaded[tab.id] = {}
          return
        }

        const parsed = JSON.parse(saved)
        loaded[tab.id] = parsed && typeof parsed === 'object' ? parsed : {}
      } catch {
        loaded[tab.id] = {}
      }
    })

    setCheckedByTab(loaded)
  }, [])

  const activeTabConfig = TABS.find((tab) => tab.id === activeTab) ?? TABS[0]
  const computedRows = useMemo(
    () => buildComputedRows(activeTabConfig.rows),
    [activeTabConfig],
  )
  const notesByIndex = useMemo(() => {
    const grouped = {}

    ;(activeTabConfig.notes ?? []).forEach((noteItem) => {
      const noteIndex = Number(noteItem.index)
      if (Number.isNaN(noteIndex) || !noteItem.note) {
        return
      }

      if (!grouped[noteIndex]) {
        grouped[noteIndex] = []
      }

      grouped[noteIndex].push(noteItem.note)
    })

    return grouped
  }, [activeTabConfig])

  const activeChecks = checkedByTab[activeTab] ?? {}

  const toggleDone = (rowIndex) => {
    setCheckedByTab((previous) => {
      const currentTabChecks = previous[activeTab] ?? {}
      const nextTabChecks = {
        ...currentTabChecks,
        [rowIndex]: !currentTabChecks[rowIndex],
      }

      const next = {
        ...previous,
        [activeTab]: nextTabChecks,
      }

      window.localStorage.setItem(
        `spawn-checklist:${activeTab}`,
        JSON.stringify(nextTabChecks),
      )

      return next
    })
  }

  return (
    <main className="app-shell">
      <header className="app-header">
        <h1>Spawn Village Development Checklist</h1>
        <p>Track your build order for 3-party and 4-party routes.</p>
      </header>

      <nav className="tabs" aria-label="Checklist tabs">
        {TABS.map((tab) => (
          <button
            key={tab.id}
            type="button"
            className={`tab-btn ${tab.id === activeTab ? 'active' : ''}`}
            onClick={() => setActiveTab(tab.id)}
          >
            {tab.label}
          </button>
        ))}
      </nav>

      <section className="table-wrap">
        <table>
          <thead>
            <tr>
              <th>Done</th>
              <th>To do</th>
              <th>Task</th>
              <th>cost</th>
              <th>reward res</th>
              <th>reward exp</th>
              <th>cp prod</th>
              <th>pop</th>
              <th>balance</th>
              <th>exp</th>
              <th>cp</th>
              <th>population</th>
            </tr>
          </thead>
          <tbody>
            {computedRows.map((row) => {
              const isDone = Boolean(activeChecks[row.index])
              const rowNotes = notesByIndex[row.index] ?? []

              return (
                <Fragment key={`${activeTab}-${row.index}-group`}>
                  {rowNotes.map((note, noteIndex) => (
                    <tr key={`${activeTab}-${row.index}-note-${noteIndex}`} className="note-row">
                      <td colSpan={12}>{note}</td>
                    </tr>
                  ))}
                  <tr key={`${activeTab}-${row.index}`} className={isDone ? 'done-row' : ''}>
                    <td>
                      <input
                        type="checkbox"
                        checked={isDone}
                        onChange={() => toggleDone(row.index)}
                        aria-label={`Mark row ${row.index + 1} as done`}
                      />
                    </td>
                    <td>{row['To do']}</td>
                    <td>{row.task}</td>
                    <td>{formatNumber(row.cost)}</td>
                    <td>{formatNumber(row['reward res'])}</td>
                    <td>{formatNumber(row['reward exp'])}</td>
                    <td>{row['cp prod']}</td>
                    <td>{row.pop}</td>
                    <td>{formatNumber(row.balance)}</td>
                    <td>{row.exp}</td>
                    <td>{row.cp}</td>
                    <td>{row.population}</td>
                  </tr>
                </Fragment>
              )
            })}
          </tbody>
        </table>
      </section>

      <div className="pwa-badge-wrap">
        <PWABadge />
      </div>
    </main>
  )
}

export default App
