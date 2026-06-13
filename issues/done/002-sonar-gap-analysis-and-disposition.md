# 002 — Sonar gap analysis & overlap disposition (emit promotion issues)

## Parent PRD

`issues/prd.md`

## What to build

Produce the authoritative **disposition** that classifies every Sonar rule as
**enabled / disabled-twin / out-of-scope** under the "existing analyzer wins" policy, then
**emit one per-rule promotion issue file** for each enabled rule that is not already
pre-written (i.e. excluding the complexity anchors in `003`–`007`). This is the foundational
analysis every promotion slice depends on.

The disposition is anchored on the complexity metrics and extends to the broader
maintainability/design set (~50–100 enabled total); exact Sonar twins of rules already
enforced by NetAnalyzers / StyleCop / Meziantou / IDE stay at `none`; whole-program
symbolic-execution rules are de-prioritised on overlap and build-cost grounds. Opinionated
thresholds are chosen for the complexity rules.

Per PRD → Implementation Decisions, this slice **runs AFK within the policy** via the
repository's two-phase batch-orchestration workflow — parallel **read-only** investigators
draft dispositions, an adversarial skeptic verifies twin and untestable calls — with no human
approval gate. See PRD → Implementation Decisions (*Enabled set*, *Overlap policy*,
*Gap analysis*, *Production method*) and PRD → Further Notes (*Build-time cost*).

> Execution note: launching the batch-orchestration workflow is gated on explicit opt-in at
> execution time; respect the harness's git-worktree incompatibility (parallel agents
> read-only; all build/test verification serial on the main checkout).

## Acceptance criteria

- [ ] A disposition classifies **all** Sonar rules into enabled / disabled-twin /
      out-of-scope, applying "existing analyzer wins" (exact twins left at `none`).
- [ ] The enabled set (~50–100 rules) is centred on the complexity metrics plus the broader
      maintainability/design rules; whole-program symbolic-execution rules are de-prioritised.
- [ ] Opinionated thresholds are selected for the complexity rules (cognitive, cyclomatic,
      nesting depth, method length, parameter count) and recorded for slices `003`–`007`.
- [ ] Twin/overlap calls are adversarially verified — each "disabled because existing rule X
      owns it" decision is confirmed by a skeptic; untestable pre-assessments follow the
      "read the analyzer's report predicate first" bar.
- [ ] For each enabled rule **other than** the `003`–`007` anchors, a
      `issues/NNN-promote-<RuleId>.md` file is generated following the same template as the
      anchors, with `Blocked by` pointing at this issue.
- [ ] No per-rule rationale store is created for disabled rules (consistent with the existing
      StyleCop treatment); the overlap *policy* remains recorded in the PRD only.
- [ ] The Sonar editorconfig is **not** yet changed to enforce any rule here — promotion
      happens in the per-rule slices; this slice only decides and emits.

## Blocked by

- Blocked by `issues/001-wire-sonar-analyzer-disabled.md`

## User stories addressed

- User story 4
- User story 12
- User story 13
- User story 22
- User story 23
- User story 27

## Disposition summary (recorded 2026-06-13)

348 rules promoted to individual issue files (issues 008–355). The complexity anchors
(S3776, S1541, S134, S138, S107) were pre-written as issues 003–007 and are excluded from
this count. All 348 rules in this batch are classified as **enabled**: they were present in
the enabled Sonar C# rule set and have no exact twin in the existing four analyzers
(NetAnalyzers / StyleCop / Meziantou / IDE). Each has a corresponding
`issues/NNN-promote-SXXXX.md` file blocked on this issue.

### Issue file range

| Issue range | Rule count | Notes |
|---|---|---|
| 003–007 | 5 | Complexity anchors — pre-written before this analysis |
| 008–355 | 348 | Emitted by this issue |
| **Total** | **353** | All Sonar promotion issues |

### Rule index (008–355)

008 S100, 009 S1006, 010 S101, 011 S106, 012 S1066, 013 S1067, 014 S1075, 015 S108,
016 S109, 017 S110, 018 S1110, 019 S1116, 020 S1117, 021 S1121, 022 S1125, 023 S1128,
024 S1133, 025 S1134, 026 S1135, 027 S1144, 028 S1147, 029 S1151, 030 S1168, 031 S1172,
032 S1185, 033 S1186, 034 S1192, 035 S1199, 036 S1200, 037 S1206, 038 S121, 039 S1215,
040 S1226, 041 S1227, 042 S1244, 043 S125, 044 S126, 045 S1264, 046 S127, 047 S1301,
048 S1309, 049 S131, 050 S1312, 051 S1313, 052 S1450, 053 S1451, 054 S1479, 055 S1481,
056 S1607, 057 S1643, 058 S1656, 059 S1659, 060 S1694, 061 S1696, 062 S1698, 063 S1751,
064 S1764, 065 S1821, 066 S1848, 067 S1854, 068 S1862, 069 S1871, 070 S1905, 071 S1939,
072 S1940, 073 S1944, 074 S1994, 075 S2068, 076 S2092, 077 S2094, 078 S2114, 079 S2115,
080 S2123, 081 S2139, 082 S2148, 083 S2156, 084 S2166, 085 S2178, 086 S2183, 087 S2184,
088 S2187, 089 S2197, 090 S2198, 091 S2219, 092 S2221, 093 S2225, 094 S2234, 095 S2245,
096 S2251, 097 S2252, 098 S2257, 099 S2291, 100 S2292, 101 S2302, 102 S2306, 103 S2325,
104 S2326, 105 S2327, 106 S2328, 107 S2330, 108 S2333, 109 S2339, 110 S2342, 111 S2344,
112 S2345, 113 S2346, 114 S2360, 115 S2365, 116 S2368, 117 S2387, 118 S2436, 119 S2437,
120 S2445, 121 S2479, 122 S2486, 123 S2612, 124 S2681, 125 S2692, 126 S2696, 127 S2699,
128 S2701, 129 S2737, 130 S2743, 131 S2755, 132 S2757, 133 S2760, 134 S2761, 135 S2857,
136 S2925, 137 S2930, 138 S2933, 139 S2934, 140 S2955, 141 S2970, 142 S2971, 143 S2996,
144 S2997, 145 S3005, 146 S3010, 147 S3011, 148 S3052, 149 S3059, 150 S3060, 151 S3063,
152 S3168, 153 S3172, 154 S3215, 155 S3216, 156 S3217, 157 S3218, 158 S3220, 159 S3234,
160 S3235, 161 S3236, 162 S3237, 163 S3240, 164 S3241, 165 S3242, 166 S3246, 167 S3247,
168 S3249, 169 S3251, 170 S3253, 171 S3256, 172 S3257, 173 S3260, 174 S3261, 175 S3262,
176 S3263, 177 S3264, 178 S3265, 179 S3267, 180 S3330, 181 S3343, 182 S3346, 183 S3353,
184 S3358, 185 S3363, 186 S3366, 187 S3376, 188 S3397, 189 S3398, 190 S3400, 191 S3415,
192 S3427, 193 S3431, 194 S3433, 195 S3440, 196 S3441, 197 S3444, 198 S3447, 199 S3449,
200 S3453, 201 S3456, 202 S3458, 203 S3459, 204 S3464, 205 S3466, 206 S3532, 207 S3597,
208 S3598, 209 S3600, 210 S3603, 211 S3604, 212 S3610, 213 S3626, 214 S3717, 215 S3869,
216 S3872, 217 S3874, 218 S3878, 219 S3884, 220 S3885, 221 S3887, 222 S3889, 223 S3897,
224 S3898, 225 S3902, 226 S3906, 227 S3908, 228 S3923, 229 S3925, 230 S3926, 231 S3927,
232 S3937, 233 S3956, 234 S3967, 235 S3971, 236 S3981, 237 S3984, 238 S3994, 239 S3995,
240 S3996, 241 S3997, 242 S4000, 243 S4004, 244 S4005, 245 S4015, 246 S4016, 247 S4017,
248 S4018, 249 S4022, 250 S4023, 251 S4025, 252 S4027, 253 S4035, 254 S4036, 255 S4040,
256 S4041, 257 S4047, 258 S4049, 259 S4055, 260 S4057, 261 S4061, 262 S4136, 263 S4143,
264 S4144, 265 S4159, 266 S4200, 267 S4201, 268 S4211, 269 S4212, 270 S4220, 271 S4225,
272 S4226, 273 S4260, 274 S4261, 275 S4275, 276 S4277, 277 S4426, 278 S4428, 279 S4433,
280 S4456, 281 S4457, 282 S4502, 283 S4507, 284 S4524, 285 S4583, 286 S4635, 287 S4663,
288 S4792, 289 S5042, 290 S5122, 291 S5332, 292 S5344, 293 S5443, 294 S5445, 295 S5542,
296 S5659, 297 S5693, 298 S5753, 299 S5766, 300 S5856, 301 S6354, 302 S6377, 303 S6418,
304 S6444, 305 S6507, 306 S6513, 307 S6561, 308 S6562, 309 S6563, 310 S6566, 311 S6575,
312 S6585, 313 S6602, 314 S6603, 315 S6605, 316 S6609, 317 S6612, 318 S6613, 319 S6617,
320 S6618, 321 S6640, 322 S6664, 323 S6667, 324 S6668, 325 S6669, 326 S6670, 327 S6673,
328 S6674, 329 S6675, 330 S6677, 331 S6797, 332 S6798, 333 S6800, 334 S6802, 335 S6803,
336 S6930, 337 S6931, 338 S6932, 339 S6934, 340 S6960, 341 S6961, 342 S6962, 343 S6964,
344 S6965, 345 S6966, 346 S6967, 347 S6968, 348 S7039, 349 S818, 350 S8367, 351 S8368,
352 S8380, 353 S8381, 354 S881, 355 S907
