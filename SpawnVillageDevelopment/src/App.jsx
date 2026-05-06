import { Fragment, useEffect, useMemo, useState } from 'react'
import { fourPartyFarmData, fourPartyFarmNote } from './data/4p-farm'
import { fourPartySimData, fourPartySimNote } from './data/4p-sim'
import { threePartyFarmData, threePartyFarmNote } from './data/3p-farm'
import { threePartySimData, threePartySimNote } from './data/3p-sim'
import { troop_cost } from './data/troop'
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

const FARM_CALC_TAB_ID = '3p-farm'
const FARM_CALC_INSERT_INDEX = 50
const FARM_CALC_SECOND_INSERT_INDEX = 61

function getNumber(value) {
  return Number(value) || 0
}

function formatNumber(value) {
  return getNumber(value).toLocaleString()
}

function formatRounded(value) {
  return Math.round(getNumber(value)).toLocaleString()
}

function buildComputedRows(rows, options = {}) {
  const {
    calcInsertIndex = -1,
    secondCalcInsertIndex = -1,
    researchCost = 0,
    trainCost = 0,
    secondTrainCost = 0,
    enableCalcRows = false,
  } = options
  let balance = INITIALS.balance
  let previousRewardRes = 0
  let exp = INITIALS.exp
  let cp = INITIALS.cp
  let population = INITIALS.population

  const applyComputedValues = (rowData) => {
    balance += previousRewardRes - getNumber(rowData.cost)
    const displayBalance = balance

    previousRewardRes = getNumber(rowData['reward res'])
    exp += getNumber(rowData['reward exp'])
    cp += getNumber(rowData['cp prod'])
    population += getNumber(rowData.pop)

    return {
      ...rowData,
      balance: displayBalance,
      exp,
      cp,
      population,
    }
  }

  const result = []

  rows.forEach((row, index) => {
    if (enableCalcRows && index === calcInsertIndex) {
      result.push(
        applyComputedValues({
          kind: 'calc-research',
          cost: researchCost,
          'reward res': 0,
          'reward exp': 0,
          'cp prod': 0,
          pop: 0,
        }),
      )

      result.push(
        applyComputedValues({
          kind: 'calc-train',
          cost: trainCost,
          'reward res': 0,
          'reward exp': 0,
          'cp prod': 0,
          pop: 0,
        }),
      )
    }

    if (enableCalcRows && index === secondCalcInsertIndex) {
      result.push(
        applyComputedValues({
          kind: 'calc-train-2',
          cost: secondTrainCost,
          'reward res': 0,
          'reward exp': 0,
          'cp prod': 0,
          pop: 0,
        }),
      )
    }

    result.push(
      applyComputedValues({
        kind: 'task',
        index,
        ...row,
      }),
    )
  })

  return result
}

function App() {
  const [activeTab, setActiveTab] = useState(TABS[0].id)
  const [checkedByTab, setCheckedByTab] = useState({})
  const [selectedUnit, setSelectedUnit] = useState(troop_cost[0]?.unit ?? '')
  const [farmUnitCount, setFarmUnitCount] = useState(10)
  const [farmUnitCountSecond, setFarmUnitCountSecond] = useState(10)

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
  const selectedTroop = useMemo(
    () => troop_cost.find((troop) => troop.unit === selectedUnit) ?? troop_cost[0],
    [selectedUnit],
  )

  const researchCost = getNumber(selectedTroop?.research)
  const trainCost = getNumber(farmUnitCount) * getNumber(selectedTroop?.train)
  const secondTrainCost = getNumber(farmUnitCountSecond) * getNumber(selectedTroop?.train)
  const breakEven = researchCost + trainCost
  const secondBreakEven = secondTrainCost
  const fullRaids = getNumber(selectedTroop?.capacity)
    ? breakEven / getNumber(selectedTroop.capacity)
    : 0
  const secondFullRaids = getNumber(selectedTroop?.capacity)
    ? secondBreakEven / getNumber(selectedTroop.capacity)
    : 0
  const perHour24 = breakEven / 24
  const perHour48 = breakEven / 48
  const secondPerHour24 = secondBreakEven / 24
  const secondPerHour48 = secondBreakEven / 48
  const showFarmCalc = activeTab === FARM_CALC_TAB_ID
  const computedRows = useMemo(
    () =>
      buildComputedRows(activeTabConfig.rows, {
        calcInsertIndex: FARM_CALC_INSERT_INDEX,
        secondCalcInsertIndex: FARM_CALC_SECOND_INSERT_INDEX,
        researchCost,
        trainCost,
        secondTrainCost,
        enableCalcRows: showFarmCalc,
      }),
    [activeTabConfig, researchCost, trainCost, secondTrainCost, showFarmCalc],
  )

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
              <th>No.</th>
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
              if (row.kind === 'calc-research') {
                return (
                  <tr key={`${activeTab}-calc-research`} className="farm-calc-row">
                    <td></td>
                    <td></td>
                    <td className="farm-calc-label">Research</td>
                    <td>
                      <select
                        value={selectedUnit}
                        onChange={(event) => setSelectedUnit(event.target.value)}
                        className="farm-calc-input farm-calc-unit-input"
                        aria-label="Select troop unit"
                      >
                        {troop_cost.map((troop) => (
                          <option key={troop.unit} value={troop.unit}>
                            {troop.unit}
                          </option>
                        ))}
                      </select>
                    </td>
                    <td className="farm-calc-value">{formatNumber(researchCost)}</td>
                    <td className="farm-calc-label">Break even</td>
                    <td className="farm-calc-label">Full raids</td>
                    <td className="farm-calc-label">Per hour (24h)</td>
                    <td className="farm-calc-label">Per hour (48h)</td>
                    <td className="farm-calc-value">{formatNumber(row.balance)}</td>
                    <td className="farm-calc-value">{formatNumber(row.exp)}</td>
                    <td className="farm-calc-value">{formatNumber(row.cp)}</td>
                    <td className="farm-calc-value">{formatNumber(row.population)}</td>
                  </tr>
                )
              }

              if (row.kind === 'calc-train') {
                return (
                  <tr key={`${activeTab}-calc-train`} className="farm-calc-row">
                    <td></td>
                    <td></td>
                    <td className="farm-calc-label">Farm units</td>
                    <td>
                      <div className="farm-calc-control">
                        <input
                          type="number"
                          min="0"
                          value={farmUnitCount}
                          onChange={(event) => setFarmUnitCount(getNumber(event.target.value))}
                          className="farm-calc-input"
                          aria-label="Farm units to make"
                        />
                        <span className="farm-calc-suffix">to make</span>
                      </div>
                    </td>
                    <td className="farm-calc-value">{formatNumber(trainCost)}</td>
                    <td className="farm-calc-value">{formatNumber(breakEven)}</td>
                    <td className="farm-calc-value">{formatRounded(fullRaids)}</td>
                    <td className="farm-calc-value">{formatRounded(perHour24)}</td>
                    <td className="farm-calc-value">{formatRounded(perHour48)}</td>
                    <td className="farm-calc-value">{formatNumber(row.balance)}</td>
                    <td className="farm-calc-value">{formatNumber(row.exp)}</td>
                    <td className="farm-calc-value">{formatNumber(row.cp)}</td>
                    <td className="farm-calc-value">{formatNumber(row.population)}</td>
                  </tr>
                )
              }

              if (row.kind === 'calc-train-2') {
                return (
                  <tr key={`${activeTab}-calc-train-2`} className="farm-calc-row">
                    <td></td>
                    <td></td>
                    <td className="farm-calc-label">Farm units</td>
                    <td>
                      <div className="farm-calc-control">
                        <input
                          type="number"
                          min="0"
                          value={farmUnitCountSecond}
                          onChange={(event) => setFarmUnitCountSecond(getNumber(event.target.value))}
                          className="farm-calc-input"
                          aria-label="Farm units to make second"
                        />
                        <span className="farm-calc-suffix">to make</span>
                      </div>
                    </td>
                    <td className="farm-calc-value">{formatNumber(secondTrainCost)}</td>
                    <td className="farm-calc-value">{formatNumber(secondBreakEven)}</td>
                    <td className="farm-calc-value">{formatRounded(secondFullRaids)}</td>
                    <td className="farm-calc-value">{formatRounded(secondPerHour24)}</td>
                    <td className="farm-calc-value">{formatRounded(secondPerHour48)}</td>
                    <td className="farm-calc-value">{formatNumber(row.balance)}</td>
                    <td className="farm-calc-value">{formatNumber(row.exp)}</td>
                    <td className="farm-calc-value">{formatNumber(row.cp)}</td>
                    <td className="farm-calc-value">{formatNumber(row.population)}</td>
                  </tr>
                )
              }

              const isDone = Boolean(activeChecks[row.index])
              const rowNotes = notesByIndex[row.index] ?? []

              return (
                <Fragment key={`${activeTab}-${row.index}-group`}>
                  {rowNotes.map((note, noteIndex) => (
                    <tr key={`${activeTab}-${row.index}-note-${noteIndex}`} className="note-row">
                      <td colSpan={13}>{note}</td>
                    </tr>
                  ))}
                  <tr key={`${activeTab}-${row.index}`} className={isDone ? 'done-row' : ''}>
                    <td>{row.index + 1}</td>
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
